namespace Restaurant_Aqa_Framework.Api.Dtos.GroupOrders;

public sealed record GuestSessionView(
    string Name,
    IReadOnlyList<ParticipantItemResponse> Items,
    decimal Total,
    bool IsSubmitted,
    DateTime? RemindedAt);
