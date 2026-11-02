using System.Text.Json;
using Microsoft.Playwright;

namespace Restaurant_Aqa_Framework.Ui;

public sealed class GuestPreOrderPage(IPage page) : BasePage(page)
{
    public ILocator Reminder => Page.GetByRole(AriaRole.Status).Filter(new() { HasText = "reminded you to submit your cart" });

    public async Task OpenAsGuestAsync(string inviteToken, string guestName, string sessionId, string sessionToken)
    {
        await NavigateToAsync("/main");
        var entries = JsonSerializer.Serialize(new Dictionary<string, string>
        {
            [$"guestPreorder:{inviteToken}:guestName"] = guestName,
            [$"guestPreorder:{inviteToken}:guestSessionId"] = sessionId,
            [$"guestPreorder:{inviteToken}:guestSessionToken"] = sessionToken,
            [$"guestPreorder:{inviteToken}:cart"] = "[]",
        });
        await Page.EvaluateAsync(
            "(entries) => { localStorage.clear(); Object.entries(JSON.parse(entries)).forEach(([key, value]) => localStorage.setItem(key, value)); }",
            entries);
        await NavigateToAsync($"/join/{inviteToken}");
    }
}
