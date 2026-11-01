using Restaurant_Aqa_Framework.Api;
using Restaurant_Aqa_Framework.Api.Dtos.GroupOrders;
using Restaurant_Aqa_Framework.Screenplay.GroupOrders;

namespace Restaurant_Aqa_Framework.Screenplay.Tasks.GroupOrders;

public sealed class JoinTheGroup(string guestName, string booking = GroupNames.Main) : ITask
{
    public async Task PerformAsAsync(Actor actor)
    {
        var group = actor.TheBooking(booking);
        var session = await actor.TheLastResponse().ExpectSuccessAsync<GuestSessionCreatedResponse>(
            await new GuestSessionsApi(actor.UsingTheApi()).JoinAsync(group.GroupReservationId, guestName),
            $"{guestName} joining {booking}");

        actor.Remember(guestName, new GuestCart(guestName, booking, session.SessionId, session.SessionToken));
    }
}
