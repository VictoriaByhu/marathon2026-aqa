using Restaurant_Aqa_Framework.Ui;

namespace Restaurant_Aqa_Framework.Screenplay.Questions.GroupOrders;

public sealed class TheRecentActivityOnThePage(string expectedEntry) : IQuestion<IReadOnlyList<string>>
{
    public async Task<IReadOnlyList<string>> AnsweredByAsync(Actor actor)
    {
        var page = new GroupPreOrderReviewPage(actor.LookingAtThePage());
        await page.ActivityTexts.Filter(new() { HasText = expectedEntry }).BecomesVisibleAsync();
        return (await page.ActivityTexts.AllInnerTextsAsync()).Select(text => text.Trim()).ToList();
    }
}
