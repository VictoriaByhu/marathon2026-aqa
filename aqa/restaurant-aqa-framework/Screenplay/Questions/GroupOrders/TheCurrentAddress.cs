namespace Restaurant_Aqa_Framework.Screenplay.Questions.GroupOrders;

public sealed class TheCurrentAddress(string expectedPart) : IQuestion<string>
{
    public async Task<string> AnsweredByAsync(Actor actor)
    {
        var page = actor.LookingAtThePage();
        try
        {
            await page.WaitForURLAsync(url => url.Contains(expectedPart, StringComparison.Ordinal));
        }
        catch (TimeoutException)
        {
            return page.Url;
        }

        return page.Url;
    }
}
