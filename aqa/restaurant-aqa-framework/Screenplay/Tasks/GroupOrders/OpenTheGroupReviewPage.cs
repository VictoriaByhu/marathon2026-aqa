using Microsoft.Playwright;
using Restaurant_Aqa_Framework.Screenplay.GroupOrders;
using Restaurant_Aqa_Framework.Ui;

namespace Restaurant_Aqa_Framework.Screenplay.Tasks.GroupOrders;

public sealed class OpenTheGroupReviewPage(bool signIn = true) : ITask
{
    public async Task PerformAsAsync(Actor actor)
    {
        if (signIn)
            await actor.AttemptsToAsync(new SignInAsTheOwnerOnTheStorefront());

        var page = new GroupPreOrderReviewPage(actor.LookingAtThePage());
        await page.NavigateAsync(actor.TheBooking().ReservationId);
        await page.ProgressTitle.WaitForAsync(new() { State = WaitForSelectorState.Visible });
    }
}
