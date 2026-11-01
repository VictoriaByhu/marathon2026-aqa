using System.Text.Json;

namespace Restaurant_Aqa_Framework.Api.Dtos.GroupOrders;

public sealed record OrderSnapshot(IReadOnlyList<OrderSnapshotItem> OrderItems, JsonElement Status);
