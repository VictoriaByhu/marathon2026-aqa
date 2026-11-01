using Restaurant_Aqa_Framework.Api;
using Restaurant_Aqa_Framework.Api.Dtos.GroupOrders;
using Restaurant_Aqa_Framework.Screenplay.GroupOrders;

namespace Restaurant_Aqa_Framework.Screenplay.Questions.GroupOrders;

public sealed class TheGroupStatus(string booking = GroupNames.Main) : IQuestion<string>
{
    public async Task<string> AnsweredByAsync(Actor actor)
    {
        var response = await new GroupOrdersApi(actor.UsingTheApi()).GetStatusAsync(actor.TheBooking(booking).GroupReservationId);
        if (!response.Ok)
            throw new InvalidOperationException($"Reading the group status failed with {response.Status}: {await response.TextAsync()}");

        return (await response.ReadAsAsync<GroupStatusResponse>()).Status;
    }
}
