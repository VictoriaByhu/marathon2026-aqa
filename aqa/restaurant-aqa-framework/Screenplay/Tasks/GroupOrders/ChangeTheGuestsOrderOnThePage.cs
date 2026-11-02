using Microsoft.Playwright;
using Restaurant_Aqa_Framework.Screenplay.GroupOrders;
using Restaurant_Aqa_Framework.Ui;
using Restaurant_Aqa_Framework.Ui.Components;

namespace Restaurant_Aqa_Framework.Screenplay.Tasks.GroupOrders;

public sealed class ChangeTheGuestsOrderOnThePage(string guestName, IReadOnlyDictionary<string, int> items) : ITask
{
    public async Task PerformAsAsync(Actor actor)
    {
        var menu = actor.Recall<GroupMenu>();
        var wanted = items
            .Select(item => (Id: menu.Dish(item.Key).Id, Name: menu.Dish(item.Key).Name, Quantity: item.Value))
            .ToList();

        await new GroupPreOrderReviewPage(actor.LookingAtThePage()).ChangeOrderAsync(guestName, wanted);
        await new ToastComponent(actor.LookingAtThePage()).Success
            .Filter(new() { HasText = $"{guestName}’s order was updated" })
            .WaitForAsync(new() { State = WaitForSelectorState.Visible });

        actor.TheCartOf(guestName).ReplaceWith(items);
    }
}
