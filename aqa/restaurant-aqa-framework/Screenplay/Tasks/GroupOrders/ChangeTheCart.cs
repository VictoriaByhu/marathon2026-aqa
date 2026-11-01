using Restaurant_Aqa_Framework.Api;
using Restaurant_Aqa_Framework.Screenplay.GroupOrders;

namespace Restaurant_Aqa_Framework.Screenplay.Tasks.GroupOrders;

public sealed class ChangeTheCart(string guestName, string dishLabel, int quantity, bool adding = true) : ITask
{
    public static ChangeTheCart Adding(string guestName, string dishLabel, int quantity) =>
        new(guestName, dishLabel, quantity);

    public static ChangeTheCart Removing(string guestName, string dishLabel, int quantity) =>
        new(guestName, dishLabel, quantity, adding: false);

    public async Task PerformAsAsync(Actor actor)
    {
        var cart = actor.TheCartOf(guestName);
        var dish = actor.Recall<GroupMenu>().Dish(dishLabel);
        var guests = new GuestSessionsApi(actor.UsingTheApi());

        var call = adding
            ? await guests.AddDishAsync(cart.SessionId, cart.SessionToken, dish.Id, quantity)
            : await guests.RemoveDishAsync(cart.SessionId, cart.SessionToken, dish.Id, quantity);
        await actor.TheLastResponse().ExpectSuccessAsync(call, $"Changing {guestName}'s cart");

        if (adding)
            cart.Add(dishLabel, quantity);
        else
            cart.Remove(dishLabel, quantity);
    }
}
