using Restaurant_Aqa_Framework.Api;
using Restaurant_Aqa_Framework.Screenplay.GroupOrders;

namespace Restaurant_Aqa_Framework.Screenplay.Tasks.GroupOrders;

public sealed class SubmitTheCart(string guestName, bool mustSucceed = true) : ITask
{
    public async Task PerformAsAsync(Actor actor)
    {
        var cart = actor.TheCartOf(guestName);
        var response = actor.TheLastResponse();
        await response.CaptureAsync(
            await new GuestSessionsApi(actor.UsingTheApi()).SubmitCartAsync(cart.SessionId, cart.SessionToken));

        if (response.Succeeded)
            cart.MarkSubmitted();
        else if (mustSucceed)
            throw new InvalidOperationException($"Submitting {guestName}'s cart failed with {response.Status}: {response.Body}");
    }
}
