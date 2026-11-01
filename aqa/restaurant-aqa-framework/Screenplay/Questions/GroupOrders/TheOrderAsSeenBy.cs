using System.Text.Json;
using Restaurant_Aqa_Framework.Api;
using Restaurant_Aqa_Framework.Api.Dtos.GroupOrders;
using Restaurant_Aqa_Framework.Api.Dtos.Orders;
using Restaurant_Aqa_Framework.Screenplay.GroupOrders;

namespace Restaurant_Aqa_Framework.Screenplay.Questions.GroupOrders;

public sealed class TheOrderAsSeenBy(Requester requester, string booking = GroupNames.Main) : IQuestion<OrderAsSeen>
{
    public async Task<OrderAsSeen> AnsweredByAsync(Actor actor)
    {
        var token = await actor.AsksForAsync(new TheCredentialsOf(requester));
        var response = await new OrdersApi(actor.UsingTheApi()).GetOrderAsync(actor.TheBooking(booking).ReservationId, token);
        var body = await response.TextAsync();
        if (!response.Ok)
            throw new InvalidOperationException($"Reading the order failed with {response.Status}: {body}");

        var order = JsonSerializer.Deserialize<OrderSnapshot>(body, JsonDefaults.Options)
            ?? throw new InvalidOperationException($"The order could not be read: {body}");
        var menu = actor.Recall<GroupMenu>();

        var items = order.OrderItems
            .GroupBy(item => menu.LabelOf(item.Dish.Id))
            .ToDictionary(group => group.Key, group => group.Sum(item => item.Quantity), StringComparer.Ordinal);

        return new OrderAsSeen(items, StatusText(order.Status));
    }

    private static string StatusText(JsonElement status) => status.ValueKind switch
    {
        JsonValueKind.String => status.GetString() ?? "",
        JsonValueKind.Number => ((OrderStatus)status.GetInt32()).ToString(),
        _ => status.GetRawText(),
    };
}
