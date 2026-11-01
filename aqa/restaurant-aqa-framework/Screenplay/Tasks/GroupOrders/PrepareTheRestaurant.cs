using System.Security.Cryptography;
using Restaurant_Aqa_Framework.Api;
using Restaurant_Aqa_Framework.Api.Dtos.Dishes;
using Restaurant_Aqa_Framework.Api.Dtos.GroupOrders;
using Restaurant_Aqa_Framework.Api.Dtos.Tables;
using Restaurant_Aqa_Framework.Screenplay.GroupOrders;
using Restaurant_Aqa_Framework.Screenplay.Questions;

namespace Restaurant_Aqa_Framework.Screenplay.Tasks.GroupOrders;

public sealed class PrepareTheRestaurant(int tableCapacity = 10) : ITask
{
    private static readonly string[] Labels = ["A", "B", "C"];

    public async Task PerformAsAsync(Actor actor)
    {
        var response = actor.TheLastResponse();
        var waiter = await actor.AsksForAsync(new TheWaiterSession());

        var location = await response.ExpectSuccessAsync<WaiterLocationResponse>(
            await new WaitersApi(actor.UsingTheApi()).GetLocationAsync(JwtClaims.SubjectOf(waiter.IdToken)),
            "Finding the waiter's location");

        var dishes = await response.ExpectSuccessAsync<List<DishListResponse>>(
            await new LocationsApi(actor.UsingTheApi()).GetDishesForLocationAsync(location.LocationId),
            "Reading the location's menu");

        var available = dishes
            .Where(dish => string.Equals(dish.State, "Available", StringComparison.OrdinalIgnoreCase))
            .OrderBy(dish => dish.Id, StringComparer.Ordinal)
            .Take(Labels.Length)
            .ToList();
        if (available.Count < 2)
            throw new InvalidOperationException(
                $"Location '{location.LocationId}' needs at least two available dishes for group pre-orders, found {available.Count}.");

        actor.Remember(new GroupMenu(available
            .Select((dish, index) => (Label: Labels[index], Dish: dish))
            .ToDictionary(pair => pair.Label, pair => pair.Dish, StringComparer.Ordinal)));

        var tables = new TablesApi(actor.UsingTheApi());
        var table = await response.ExpectSuccessAsync<TableResponse>(
            await tables.CreateTableAsync(new CreateTableRequest(
                location.LocationId, tableCapacity, RandomNumberGenerator.GetInt32(10_000, 99_999))),
            "Adding a table for the group");

        actor.Remember(new RestaurantTable(table.Id, location.LocationId, tableCapacity));
        actor.TheCleanup().TrackDelete(
            $"table:{table.Id}",
            () => tables.DeleteTableAsync(table.Id, bearerToken: waiter.IdToken));
    }
}
