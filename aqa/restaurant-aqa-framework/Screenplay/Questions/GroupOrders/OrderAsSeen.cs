namespace Restaurant_Aqa_Framework.Screenplay.Questions.GroupOrders;

public sealed record OrderAsSeen(IReadOnlyDictionary<string, int> Items, string Status);
