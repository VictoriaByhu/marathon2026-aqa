using Restaurant_Aqa_Framework.Ui;

namespace Restaurant_Aqa_Framework.Screenplay.Questions.GroupOrders;

public sealed class TheWaiterPreOrderLines(int expectedCount) : IQuestion<IReadOnlyDictionary<string, int>>
{
    public async Task<IReadOnlyDictionary<string, int>> AnsweredByAsync(Actor actor)
    {
        var page = new WaiterPreOrderPage(actor.LookingAtThePage());

        await page.Lines.SettlesOnCountAsync(expectedCount);

        var lines = new Dictionary<string, int>(StringComparer.Ordinal);
        foreach (var line in await page.Lines.AllAsync())
        {
            var name = (await WaiterPreOrderPage.DishName(line).InnerTextAsync()).Trim();
            lines[name] = Money.ParseCount(await WaiterPreOrderPage.Quantity(line).InnerTextAsync());
        }

        return lines;
    }
}
