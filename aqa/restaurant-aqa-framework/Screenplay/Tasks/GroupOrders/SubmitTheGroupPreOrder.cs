using Restaurant_Aqa_Framework.Api;
using Restaurant_Aqa_Framework.Screenplay.GroupOrders;
using Restaurant_Aqa_Framework.Screenplay.Questions.GroupOrders;

namespace Restaurant_Aqa_Framework.Screenplay.Tasks.GroupOrders;

public sealed class SubmitTheGroupPreOrder(
    Requester requester = Requester.Owner,
    string? groupReservationId = null,
    string? reservationId = null,
    bool mustSucceed = false) : ITask
{
    public static SubmitTheGroupPreOrder ByTheOwner() => new(mustSucceed: true);

    public static SubmitTheGroupPreOrder ForAGroupThatDoesNotExist() => new(groupReservationId: Guid.NewGuid().ToString());

    public static SubmitTheGroupPreOrder ForAReservationThatDoesNotExist() => new(reservationId: Guid.NewGuid().ToString());

    public async Task PerformAsAsync(Actor actor)
    {
        var booking = actor.TheBooking();
        var token = await actor.AsksForAsync(new TheCredentialsOf(requester));
        var response = actor.TheLastResponse();

        await response.CaptureAsync(await new GroupOrdersApi(actor.UsingTheApi()).SubmitAsync(
            groupReservationId ?? booking.GroupReservationId,
            reservationId ?? booking.ReservationId,
            token));

        if (mustSucceed && !response.Succeeded)
            throw new InvalidOperationException($"Submitting the group pre-order failed with {response.Status}: {response.Body}");
    }
}
