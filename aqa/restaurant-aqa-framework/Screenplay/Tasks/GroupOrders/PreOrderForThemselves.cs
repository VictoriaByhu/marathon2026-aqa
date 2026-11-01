using Restaurant_Aqa_Framework.Api;
using Restaurant_Aqa_Framework.Api.Dtos.Orders;
using Restaurant_Aqa_Framework.Screenplay.GroupOrders;

namespace Restaurant_Aqa_Framework.Screenplay.Tasks.GroupOrders;

public sealed class PreOrderForThemselves(IReadOnlyDictionary<string, int> items, string booking = GroupNames.Main) : ITask
{
    public async Task PerformAsAsync(Actor actor)
    {
        var menu = actor.Recall<GroupMenu>();
        var lines = items.Select(item => new OrderItemRequest(menu.Dish(item.Key).Id, item.Value)).ToList();

        await actor.TheLastResponse().ExpectSuccessAsync(
            await new OrdersApi(actor.UsingTheApi()).CreateOrderAsync(
                new CreateOrderRequest(actor.TheBooking(booking).ReservationId, lines),
                actor.TheOwner().IdToken),
            "The owner's own pre-order");

        actor.Remember(new OwnPreOrder(items));
    }
}
