namespace Restaurant_Aqa_Framework.Api.Dtos.GroupOrders;

public sealed record ReplaceGuestDishesRequest(IReadOnlyList<GuestDishRequest> Items);
