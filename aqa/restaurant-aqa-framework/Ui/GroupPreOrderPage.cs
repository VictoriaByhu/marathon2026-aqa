using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Restaurant_Aqa_Framework.Ui.Components;

namespace Restaurant_Aqa_Framework.Ui;

public sealed partial class GroupPreOrderPage(IPage page) : BasePage(page)
{
    public ILocator Heading => Page.GetByRole(AriaRole.Heading, new() { Name = "Group Pre-Order", Exact = true });
    public ILocator StatusBadge => Page.Locator(".status-badge");
    public ILocator Participants => Page.Locator("app-participant-card");
    public ILocator GuestCards => Participants.Filter(new()
    {
        Has = Page.Locator("h2.title").Filter(new() { HasTextRegex = GuestTitlePattern() }),
    });
    public ILocator GrandTotal => Page.Locator(".grand-total-value");
    public ILocator SubmitButton => Page.GetByRole(AriaRole.Button, new() { Name = "Submit Pre-Order", Exact = true });
    public ILocator NothingAddedYet => Page.Locator("p.state").Filter(new() { HasTextRegex = NothingAddedPattern() });
    public ILocator SentNote => Page.GetByRole(AriaRole.Status).Filter(new() { HasText = "has been sent to the restaurant" });
    public ILocator NotFound => Page.GetByRole(AriaRole.Alert).Filter(new() { HasText = "Group reservation not found" });

    public ParticipantCardComponent Participant(string title) =>
        new(Participants.Filter(new()
        {
            Has = Page.GetByRole(AriaRole.Heading, new() { Name = title, Exact = true }),
        }));

    public ILocator RemoveButtonFor(string guestName) =>
        Page.GetByRole(AriaRole.Button, new() { Name = $"Remove {guestName}", Exact = true });

    public ILocator Dialog(string title) => Page.GetByRole(AriaRole.Dialog, new() { Name = title, Exact = true });

    public Task NavigateAsync(string reservationId) => NavigateToAsync($"/reservations/{reservationId}/group-order");

    public async Task RemoveGuestAsync(string guestName)
    {
        await RemoveButtonFor(guestName).ClickAsync();
        var dialog = Dialog("Remove Guest");
        await dialog.GetByRole(AriaRole.Button, new() { Name = "Remove Guest", Exact = true }).ClickAsync();
        await dialog.WaitForAsync(new() { State = WaitForSelectorState.Hidden });
    }

    public async Task SubmitAsync()
    {
        await SubmitButton.ClickAsync();
        var dialog = Dialog("Submit Pre-Order");
        await dialog.GetByRole(AriaRole.Button, new() { Name = "Submit", Exact = true }).ClickAsync();
        await dialog.WaitForAsync(new() { State = WaitForSelectorState.Hidden });
    }

    [GeneratedRegex("^Guest: ")]
    private static partial Regex GuestTitlePattern();

    [GeneratedRegex("added any dishes yet")]
    private static partial Regex NothingAddedPattern();
}
