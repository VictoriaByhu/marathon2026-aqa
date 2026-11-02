using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Playwright;

namespace Restaurant_Aqa_Framework.Ui;

public sealed partial class GroupPreOrderReviewPage(IPage page) : BasePage(page)
{
    public ILocator Heading => Page.GetByRole(AriaRole.Heading, new() { Name = "Group Pre-Order Cart", Exact = true });
    public ILocator ProgressTitle => Page.Locator("#progress-title");
    public ILocator RemindPendingButton => Page.GetByRole(AriaRole.Button, new() { NameRegex = RemindPendingPattern() });
    public ILocator ActivityToggle => Page.Locator("button.activity-toggle");
    public ILocator ActivityList => Page.GetByRole(AriaRole.List, new() { Name = "Recent activity" });
    public ILocator ActivityTexts => ActivityList.Locator(".activity-text");

    public Task NavigateAsync(string reservationId) => NavigateToAsync($"/reservations/{reservationId}/group-order/review");

    public ILocator GuestCard(string guestName) =>
        Page.Locator("article.person-card").Filter(new()
        {
            Has = Page.GetByRole(AriaRole.Button, new() { Name = $"Toggle {guestName}’s order", Exact = true }),
        });

    public static ILocator DishRows(ILocator card) => card.Locator("li.dish-row");
    public static ILocator DishName(ILocator row) => row.Locator(".dish-name");
    public static ILocator DishQuantity(ILocator row) => row.Locator(".muted.small");
    public static ILocator RemindedNote(ILocator card) => card.Locator(".reminded-note");

    public ILocator EditDialog(string guestName) =>
        Page.GetByRole(AriaRole.Dialog, new() { Name = $"Edit {guestName}’s order", Exact = true });

    public async Task OpenActionsAsync(string guestName)
    {
        await Page.GetByRole(AriaRole.Button, new() { Name = $"Actions for {guestName}", Exact = true }).ClickAsync();
    }

    public async Task ChooseActionAsync(string guestName, string action)
    {
        await OpenActionsAsync(guestName);
        await Page.GetByRole(AriaRole.Menuitem, new() { Name = action, Exact = true }).ClickAsync();
    }

    public async Task ChangeOrderAsync(string guestName, IReadOnlyList<(string Id, string Name, int Quantity)> wanted)
    {
        await ChooseActionAsync(guestName, $"Edit {guestName}’s order");
        var dialog = EditDialog(guestName);
        await dialog.WaitForAsync();

        var present = new List<string>();
        foreach (var name in await dialog.Locator("li.line .name").AllInnerTextsAsync())
            present.Add(name.Trim());

        foreach (var name in present.Where(name => wanted.All(dish => dish.Name != name)))
            await dialog.GetByRole(AriaRole.Button, new() { Name = $"Remove {name}", Exact = true }).ClickAsync();

        foreach (var dish in wanted)
        {
            if (!present.Contains(dish.Name))
            {
                await dialog.Locator("select").SelectOptionAsync(dish.Id);
                await dialog.GetByRole(AriaRole.Button, new() { Name = "Add", Exact = true }).ClickAsync();
            }

            var quantity = dialog.Locator("li.line").Filter(new() { HasText = dish.Name }).Locator(".quantity");
            var current = int.Parse((await quantity.InnerTextAsync()).Trim(), CultureInfo.InvariantCulture);
            while (current != dish.Quantity)
            {
                var step = current < dish.Quantity ? "Increase" : "Decrease";
                await dialog.GetByRole(AriaRole.Button, new() { Name = $"{step} {dish.Name}", Exact = true }).ClickAsync();
                current += current < dish.Quantity ? 1 : -1;
                await Assertions.Expect(quantity).ToHaveTextAsync(current.ToString(CultureInfo.InvariantCulture));
            }
        }

        await dialog.GetByRole(AriaRole.Button, new() { Name = "Save changes", Exact = true }).ClickAsync();
        await dialog.WaitForAsync(new() { State = WaitForSelectorState.Hidden });
    }

    [GeneratedRegex(@"^Remind \d+ pending$")]
    private static partial Regex RemindPendingPattern();
}
