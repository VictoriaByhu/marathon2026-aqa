using Restaurant_Aqa_Framework.Api;
using Restaurant_Aqa_Framework.Screenplay.GroupOrders;
using Restaurant_Aqa_Framework.Screenplay.Questions.GroupOrders;

namespace Restaurant_Aqa_Framework.Screenplay.Tasks.GroupOrders;

public sealed class ViewTheGroup(Requester requester = Requester.Owner, string? groupReservationId = null) : ITask
{
    public static ViewTheGroup ThatDoesNotExist() => new(Requester.Owner, Guid.NewGuid().ToString());

    public async Task PerformAsAsync(Actor actor)
    {
        var token = await actor.AsksForAsync(new TheCredentialsOf(requester));
        var group = groupReservationId ?? actor.TheBooking().GroupReservationId;

        await actor.TheLastResponse().CaptureAsync(
            await new GroupOrdersApi(actor.UsingTheApi()).GetSessionsAsync(group, token));
    }
}
