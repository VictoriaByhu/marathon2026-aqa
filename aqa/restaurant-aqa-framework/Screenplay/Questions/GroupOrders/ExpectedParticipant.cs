namespace Restaurant_Aqa_Framework.Screenplay.Questions.GroupOrders;

public sealed record ExpectedParticipant(
    string Name,
    string Type,
    IReadOnlyDictionary<string, int> Items,
    decimal Subtotal,
    bool IsSubmitted);
