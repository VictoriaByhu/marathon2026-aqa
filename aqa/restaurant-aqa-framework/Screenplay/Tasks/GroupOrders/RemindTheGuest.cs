using Restaurant_Aqa_Framework.Api;
using Restaurant_Aqa_Framework.Screenplay.GroupOrders;
using Restaurant_Aqa_Framework.Screenplay.Questions.GroupOrders;

namespace Restaurant_Aqa_Framework.Screenplay.Tasks.GroupOrders;

public sealed class RemindTheGuest(string guestName, Requester requester = Requester.Owner) : ITask
{
    public async Task PerformAsAsync(Actor actor)
    {
        var cart = actor.TheCartOf(guestName);
        var token = await actor.AsksForAsync(new TheCredentialsOf(requester));

        await actor.TheLastResponse().CaptureAsync(await new GroupOrdersApi(actor.UsingTheApi()).RemindGuestAsync(
            actor.TheBooking(cart.Booking).GroupReservationId, cart.SessionId, token));
    }
}
