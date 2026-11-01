using System.Security.Cryptography;
using Restaurant_Aqa_Framework.Api;
using Restaurant_Aqa_Framework.Api.Dtos.GroupOrders;
using Restaurant_Aqa_Framework.Api.Dtos.Reservations;
using Restaurant_Aqa_Framework.Screenplay.GroupOrders;

namespace Restaurant_Aqa_Framework.Screenplay.Tasks.GroupOrders;

public sealed class BookAGroupTable(int guests, string booking = GroupNames.Main) : ITask
{
    public async Task PerformAsAsync(Actor actor)
    {
        var owner = actor.TheOwner();
        var table = actor.Recall<RestaurantTable>();
        var response = actor.TheLastResponse();
        var reservations = new ReservationsApi(actor.UsingTheApi());
        var groups = new GroupOrdersApi(actor.UsingTheApi());

        var startTime = StartTimeFor(actor, actor.RecallAll<GroupBooking>().Count);
        var reservation = await response.ExpectSuccessAsync<BookedReservation>(
            await reservations.CreateReservationAsync(
                new CreateReservationRequest(table.Id, startTime, startTime.AddHours(2), guests),
                owner.IdToken),
            "Booking a table for the group");

        actor.TheCleanup().TrackDelete(
            $"reservation:{reservation.Id}",
            () => reservations.CancelReservationAsync(reservation.Id, owner.IdToken));

        var invite = await response.ExpectSuccessAsync<InviteTokenResponse>(
            await groups.GetInviteTokenAsync(reservation.Id, owner.IdToken),
            "Getting the group invitation");
        var group = await response.ExpectSuccessAsync<GroupInviteResponse>(
            await groups.GetInviteAsync(invite.Token),
            "Opening the group invitation");

        actor.Remember(booking, new GroupBooking(booking, reservation.Id, group.GroupReservationId, guests, startTime));
    }

    private static DateTime StartTimeFor(Actor actor, int earlierBookings)
    {
        if (!actor.TryRecall<BookingDay>(out var day))
        {
            day = new BookingDay(DateTime.SpecifyKind(
                DateTime.UtcNow.Date.AddDays(14 + RandomNumberGenerator.GetInt32(0, 60)), DateTimeKind.Utc));
            actor.Remember(day);
        }

        return day.Date.AddHours(10 + (earlierBookings * 4));
    }
}
