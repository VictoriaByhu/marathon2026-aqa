using System.Text.Json;
using Microsoft.Playwright;

namespace Restaurant_Aqa_Framework.Ui;

public sealed class WaiterPreOrderPage(IPage page) : BasePage(page)
{
    private const string StorageKey = "waiterPreOrderData";

    public ILocator Lines => Page.Locator("app-dish-card.pre-order-dish");

    public static ILocator DishName(ILocator line) => line.Locator(".dish-name");
    public static ILocator Quantity(ILocator line) => line.Locator(".pre-order-quantity");

    public async Task OpenForAsync(string reservationId, string customerName)
    {
        var state = JsonSerializer.Serialize(new Dictionary<string, string>
        {
            ["reservationId"] = reservationId,
            ["createdByName"] = customerName,
            ["createdByRole"] = "Customer",
            ["tableName"] = "Group table",
        });
        await Page.EvaluateAsync("([key, value]) => sessionStorage.setItem(key, value)", new[] { StorageKey, state });
        await NavigateToAsync("/waiter-pre-order");
    }
}
