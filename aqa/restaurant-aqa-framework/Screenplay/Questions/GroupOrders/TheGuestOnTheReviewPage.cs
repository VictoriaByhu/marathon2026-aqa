using Microsoft.Playwright;
using Restaurant_Aqa_Framework.Ui;

namespace Restaurant_Aqa_Framework.Screenplay.Questions.GroupOrders;

public sealed record GuestOnTheReviewPage(IReadOnlyDictionary<string, int> QuantitiesByDish, bool ShowsReminder);

public sealed class TheGuestOnTheReviewPage(string guestName, int expectedDishes) : IQuestion<GuestOnTheReviewPage>
{
    public async Task<GuestOnTheReviewPage> AnsweredByAsync(Actor actor)
    {
        var card = new GroupPreOrderReviewPage(actor.LookingAtThePage()).GuestCard(guestName);
        await card.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await GroupPreOrderReviewPage.DishRows(card).SettlesOnCountAsync(expectedDishes);

        var quantities = new Dictionary<string, int>(StringComparer.Ordinal);
        foreach (var row in await GroupPreOrderReviewPage.DishRows(card).AllAsync())
        {
            var name = (await GroupPreOrderReviewPage.DishName(row).InnerTextAsync()).Trim();
            quantities[name] = Money.ParseCount(await GroupPreOrderReviewPage.DishQuantity(row).InnerTextAsync());
        }

        return new GuestOnTheReviewPage(quantities, await GroupPreOrderReviewPage.RemindedNote(card).IsVisibleAsync());
    }
}
