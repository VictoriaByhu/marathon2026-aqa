using Restaurant_Aqa_Framework.Screenplay.GroupOrders;
using Restaurant_Aqa_Framework.Ui;

namespace Restaurant_Aqa_Framework.Screenplay.Tasks.GroupOrders;

public sealed class OpenTheGuestsPreOrderPage(string guestName) : ITask
{
    public async Task PerformAsAsync(Actor actor)
    {
        var cart = actor.TheCartOf(guestName);
        await new GuestPreOrderPage(actor.LookingAtThePage()).OpenAsGuestAsync(
            actor.TheBooking(cart.Booking).InviteToken, guestName, cart.SessionId, cart.SessionToken);
    }
}
