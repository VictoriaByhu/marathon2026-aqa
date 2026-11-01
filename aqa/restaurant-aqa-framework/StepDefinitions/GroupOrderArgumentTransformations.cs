using Restaurant_Aqa_Framework.Screenplay.GroupOrders;

namespace Restaurant_Aqa_Framework.StepDefinitions;

public enum Refusal
{
    NotTheirGroup,
    NotSignedIn,
}

[Binding]
public static class GroupOrderArgumentTransformations
{
    [StepArgumentTransformation(Name = "someone", Regex = "(another customer|the waiter|a guest of the group|an anonymous visitor)")]
    public static Requester ToRequester(string value) => value switch
    {
        "another customer" => Requester.AnotherCustomer,
        "the waiter" => Requester.Waiter,
        "a guest of the group" => Requester.Guest,
        "an anonymous visitor" => Requester.Anonymous,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, null),
    };

    [StepArgumentTransformation(Name = "reason", Regex = "(it is not their group|they are not signed in)")]
    public static Refusal ToRefusal(string value) => value switch
    {
        "it is not their group" => Refusal.NotTheirGroup,
        "they are not signed in" => Refusal.NotSignedIn,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, null),
    };

    public static IReadOnlyDictionary<string, int> ToDishQuantities(this DataTable table) =>
        table.Rows.ToDictionary(
            row => row["dish"].Trim(),
            row => int.Parse(row["quantity"], System.Globalization.CultureInfo.InvariantCulture),
            StringComparer.Ordinal);

    public static string Describe(this IReadOnlyDictionary<string, int> items) =>
        items.Count == 0
            ? "nothing"
            : string.Join(", ", items.OrderBy(item => item.Key, StringComparer.Ordinal).Select(item => $"{item.Key} x{item.Value}"));
}
