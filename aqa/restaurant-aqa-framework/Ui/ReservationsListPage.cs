using Microsoft.Playwright;

namespace Restaurant_Aqa_Framework.Ui;

public sealed class ReservationsListPage(IPage page) : BasePage(page)
{
    public ILocator Cards => Page.Locator("app-card-reservation");

    public ILocator GroupPreOrderButton => Cards.GetByRole(AriaRole.Button, new() { Name = "Group pre-order", Exact = true });

    public Task NavigateAsync() => NavigateToAsync("/reservations");
}
