using Microsoft.Playwright;
using Restaurant_Aqa_Framework.Screenplay.GroupOrders;
using Restaurant_Aqa_Framework.Ui;

namespace Restaurant_Aqa_Framework.Screenplay.Tasks.GroupOrders;

public sealed class OpenTheGroupPreOrderPage(string booking = GroupNames.Main) : ITask
{
    public async Task PerformAsAsync(Actor actor)
    {
        var page = new GroupPreOrderPage(actor.LookingAtThePage());
        await page.NavigateAsync(actor.TheBooking(booking).ReservationId);
        await page.Heading.WaitForAsync();
        await page.GrandTotal.WaitForAsync(new() { State = WaitForSelectorState.Visible });
    }
}
