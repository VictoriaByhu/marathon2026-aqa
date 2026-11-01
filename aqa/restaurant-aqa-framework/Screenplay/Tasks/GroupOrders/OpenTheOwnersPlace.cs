using Restaurant_Aqa_Framework.Api;
using Restaurant_Aqa_Framework.Api.Dtos.GroupOrders;
using Restaurant_Aqa_Framework.Screenplay.GroupOrders;

namespace Restaurant_Aqa_Framework.Screenplay.Tasks.GroupOrders;

public sealed class OpenTheOwnersPlace(string booking = GroupNames.Main) : ITask
{
    public async Task PerformAsAsync(Actor actor)
    {
        var session = await actor.TheLastResponse().ExpectSuccessAsync<OwnerSessionResponse>(
            await new GroupOrdersApi(actor.UsingTheApi()).GetOwnerSessionIdAsync(
                actor.TheBooking(booking).ReservationId, actor.TheOwner().IdToken),
            "Opening the owner's place in the group");

        actor.Remember(new OwnerPlace(booking, session.GuestSessionId));
    }
}
