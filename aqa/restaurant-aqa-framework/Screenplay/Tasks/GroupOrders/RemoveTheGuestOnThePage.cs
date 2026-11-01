using Microsoft.Playwright;
using Restaurant_Aqa_Framework.Screenplay.GroupOrders;
using Restaurant_Aqa_Framework.Ui;
using Restaurant_Aqa_Framework.Ui.Components;

namespace Restaurant_Aqa_Framework.Screenplay.Tasks.GroupOrders;

public sealed class RemoveTheGuestOnThePage(string guestName) : ITask
{
    public async Task PerformAsAsync(Actor actor)
    {
        var browser = actor.LookingAtThePage();
        var page = new GroupPreOrderPage(browser);

        await page.RemoveGuestAsync(guestName);
        await new ToastComponent(browser).Success
            .Filter(new() { HasText = $"{guestName} was removed from the group" })
            .WaitForAsync(new() { State = WaitForSelectorState.Visible });

        actor.TheCartOf(guestName).MarkRemoved();
    }
}
