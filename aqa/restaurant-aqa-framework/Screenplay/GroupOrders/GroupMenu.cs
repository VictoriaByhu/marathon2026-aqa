using Restaurant_Aqa_Framework.Api.Dtos.Dishes;

namespace Restaurant_Aqa_Framework.Screenplay.GroupOrders;

public sealed class GroupMenu(IReadOnlyDictionary<string, DishListResponse> dishesByLabel)
{
    public IReadOnlyDictionary<string, DishListResponse> DishesByLabel { get; } = dishesByLabel;

    public DishListResponse Dish(string label) =>
        DishesByLabel.TryGetValue(label, out var dish)
            ? dish
            : throw new InvalidOperationException(
                $"The menu has no dish labelled '{label}'. Labels in use: {string.Join(", ", DishesByLabel.Keys)}.");

    public string LabelOf(string dishId) =>
        DishesByLabel.FirstOrDefault(pair => string.Equals(pair.Value.Id, dishId, StringComparison.Ordinal)).Key
            ?? throw new InvalidOperationException($"Dish '{dishId}' is not one of the dishes this scenario ordered.");

    public decimal PriceOf(IReadOnlyDictionary<string, int> items) =>
        items.Sum(item => Dish(item.Key).Price * item.Value);
}
