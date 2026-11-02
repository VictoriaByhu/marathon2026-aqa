using Microsoft.Playwright;
using Restaurant_Aqa_Framework.Ui;
using Restaurant_Aqa_Framework.Ui.Components;

namespace Restaurant_Aqa_Framework.Screenplay.Tasks.GroupOrders;

public sealed class RemindTheGuestOnThePage(string guestName) : ITask
{
    public async Task PerformAsAsync(Actor actor)
    {
        await new GroupPreOrderReviewPage(actor.LookingAtThePage()).ChooseActionAsync(guestName, "Send reminder");
        await new ToastComponent(actor.LookingAtThePage()).Success
            .Filter(new() { HasText = $"Reminder sent to {guestName}" })
            .WaitForAsync(new() { State = WaitForSelectorState.Visible });
    }
}
