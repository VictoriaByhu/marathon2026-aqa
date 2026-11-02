using Restaurant_Aqa_Framework.Api;
using Restaurant_Aqa_Framework.Api.Dtos.GroupOrders;
using Restaurant_Aqa_Framework.Screenplay.GroupOrders;
using Restaurant_Aqa_Framework.Screenplay.Questions.GroupOrders;

namespace Restaurant_Aqa_Framework.Screenplay.Tasks.GroupOrders;

public sealed class ChangeTheGuestsOrder : ITask
{
    private readonly string _guestName;
    private readonly IReadOnlyDictionary<string, int> _items;
    private readonly Requester _requester;
    private readonly string? _foreignDishId;

    private ChangeTheGuestsOrder(string guestName, IReadOnlyDictionary<string, int> items, Requester requester, string? foreignDishId)
    {
        _guestName = guestName;
        _items = items;
        _requester = requester;
        _foreignDishId = foreignDishId;
    }

    public static ChangeTheGuestsOrder To(string guestName, IReadOnlyDictionary<string, int> items, Requester requester = Requester.Owner) =>
        new(guestName, items, requester, null);

    public static ChangeTheGuestsOrder ToNothing(string guestName) =>
        new(guestName, new Dictionary<string, int>(), Requester.Owner, null);

    public static ChangeTheGuestsOrder WithADishFromAnotherRestaurant(string guestName) =>
        new(guestName, new Dictionary<string, int>(), Requester.Owner, $"no-such-dish-{Guid.NewGuid():N}");

    public async Task PerformAsAsync(Actor actor)
    {
        var cart = actor.TheCartOf(_guestName);
        var menu = actor.Recall<GroupMenu>();
        var token = await actor.AsksForAsync(new TheCredentialsOf(_requester));

        var lines = _items.Select(item => new GuestDishRequest(menu.Dish(item.Key).Id, item.Value)).ToList();
        if (_foreignDishId is not null)
            lines.Add(new GuestDishRequest(_foreignDishId, 1));

        var response = actor.TheLastResponse();
        await response.CaptureAsync(await new GroupOrdersApi(actor.UsingTheApi()).ReplaceGuestDishesAsync(
            actor.TheBooking(cart.Booking).GroupReservationId, cart.SessionId, lines, token));

        if (response.Succeeded)
            cart.ReplaceWith(_items);
    }
}
