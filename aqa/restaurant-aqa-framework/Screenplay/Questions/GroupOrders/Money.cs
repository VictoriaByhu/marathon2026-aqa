using System.Globalization;
using System.Text.RegularExpressions;

namespace Restaurant_Aqa_Framework.Screenplay.Questions.GroupOrders;

public static partial class Money
{
    public static decimal Parse(string text)
    {
        var match = AmountPattern().Match(text);
        if (!match.Success)
            throw new FormatException($"'{text}' does not contain an amount.");

        return decimal.Parse(match.Value.Replace(",", "", StringComparison.Ordinal), NumberStyles.Number, CultureInfo.InvariantCulture);
    }

    public static int ParseCount(string text)
    {
        var match = CountPattern().Match(text);
        if (!match.Success)
            throw new FormatException($"'{text}' does not contain a quantity.");

        return int.Parse(match.Value, CultureInfo.InvariantCulture);
    }

    [GeneratedRegex(@"-?[0-9][0-9,]*(\.[0-9]+)?")]
    private static partial Regex AmountPattern();

    [GeneratedRegex("[0-9]+")]
    private static partial Regex CountPattern();
}
