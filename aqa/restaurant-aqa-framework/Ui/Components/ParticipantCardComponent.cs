using Microsoft.Playwright;

namespace Restaurant_Aqa_Framework.Ui.Components;

public sealed class ParticipantCardComponent(ILocator root)
{
    public ILocator Title => root.Locator("h2.title");
    public ILocator Status => root.Locator(".chip");
    public ILocator Items => root.Locator("li.item");
    public ILocator Subtotal => root.Locator(".subtotal span").Last;
    public ILocator NoDishes => root.Locator(".empty");

    public static ILocator ItemName(ILocator item) => item.Locator(".name");
    public static ILocator ItemQuantity(ILocator item) => item.Locator(".qty");
    public static ILocator ItemPrice(ILocator item) => item.Locator(".price");
}
