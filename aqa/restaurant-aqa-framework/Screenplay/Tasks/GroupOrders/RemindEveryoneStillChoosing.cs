using Restaurant_Aqa_Framework.Api;
using Restaurant_Aqa_Framework.Screenplay.GroupOrders;
using Restaurant_Aqa_Framework.Screenplay.Questions.GroupOrders;

namespace Restaurant_Aqa_Framework.Screenplay.Tasks.GroupOrders;

public sealed class RemindEveryoneStillChoosing : ITask
{
    public async Task PerformAsAsync(Actor actor)
    {
        var token = await actor.AsksForAsync(new TheCredentialsOf(Requester.Owner));

        await actor.TheLastResponse().CaptureAsync(await new GroupOrdersApi(actor.UsingTheApi()).RemindPendingAsync(
            actor.TheBooking().GroupReservationId, token));
    }
}
