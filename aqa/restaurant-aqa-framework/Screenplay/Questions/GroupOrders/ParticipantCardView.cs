namespace Restaurant_Aqa_Framework.Screenplay.Questions.GroupOrders;

public sealed record ParticipantCardView(
    string Title,
    string Status,
    IReadOnlyDictionary<string, int> QuantitiesByDish,
    decimal Subtotal,
    bool ShowsNoDishes);
