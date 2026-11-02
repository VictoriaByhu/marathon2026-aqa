namespace Restaurant_Aqa_Framework.Api.Dtos.GroupOrders;

public sealed record GroupActivityEntry(string Type, string? GuestName, string? DishName, int? Quantity, DateTime OccurredAt);
