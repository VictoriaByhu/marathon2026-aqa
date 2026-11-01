using Microsoft.Playwright;

namespace Restaurant_Aqa_Framework.Ui;

public sealed class StorefrontSession(IPage page) : BasePage(page)
{
    public async Task SignOutAsync()
    {
        await Page.EvaluateAsync("() => { localStorage.clear(); sessionStorage.clear(); }");
        await NavigateToAsync("/main");
    }
}
