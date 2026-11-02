using Restaurant_Aqa_Framework.Api;
using Restaurant_Aqa_Framework.Api.Dtos.GroupOrders;
using Restaurant_Aqa_Framework.Screenplay.GroupOrders;

namespace Restaurant_Aqa_Framework.Screenplay.Questions.GroupOrders;

public sealed class TheCartAsSeenByTheGuest(string guestName) : IQuestion<GuestSessionView>
{
    public async Task<GuestSessionView> AnsweredByAsync(Actor actor)
    {
        var cart = actor.TheCartOf(guestName);
        var response = await new GuestSessionsApi(actor.UsingTheApi()).GetSessionAsync(cart.SessionId, cart.SessionToken);
        if (!response.Ok)
            throw new InvalidOperationException($"{guestName} could not open the cart: {response.Status} {await response.TextAsync()}");

        return await response.ReadAsAsync<GuestSessionView>();
    }
}
