namespace Restaurant_Aqa_Framework.Api.Dtos.GroupOrders;

public sealed record GroupSessionsResponse(IReadOnlyList<ParticipantResponse> Participants, decimal Total);
