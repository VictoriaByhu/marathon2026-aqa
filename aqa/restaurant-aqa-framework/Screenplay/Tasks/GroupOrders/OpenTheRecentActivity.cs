using Microsoft.Playwright;
using Restaurant_Aqa_Framework.Ui;

namespace Restaurant_Aqa_Framework.Screenplay.Tasks.GroupOrders;

public sealed class OpenTheRecentActivity : ITask
{
    public async Task PerformAsAsync(Actor actor)
    {
        var page = new GroupPreOrderReviewPage(actor.LookingAtThePage());
        await page.ActivityToggle.ClickAsync();
        await page.ActivityList.WaitForAsync(new() { State = WaitForSelectorState.Visible });
    }
}
