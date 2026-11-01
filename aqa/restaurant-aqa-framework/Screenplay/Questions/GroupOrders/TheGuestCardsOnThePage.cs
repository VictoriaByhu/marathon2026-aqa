using Microsoft.Playwright;
using Restaurant_Aqa_Framework.Ui;
using Restaurant_Aqa_Framework.Ui.Components;

namespace Restaurant_Aqa_Framework.Screenplay.Questions.GroupOrders;

public sealed class TheGuestCardsOnThePage(int expectedCount) : IQuestion<IReadOnlyList<ParticipantCardView>>
{
    public async Task<IReadOnlyList<ParticipantCardView>> AnsweredByAsync(Actor actor)
    {
        var page = new GroupPreOrderPage(actor.LookingAtThePage());
        await page.GrandTotal.WaitForAsync(new() { State = WaitForSelectorState.Visible });

        await page.GuestCards.SettlesOnCountAsync(expectedCount);

        var cards = new List<ParticipantCardView>();
        foreach (var root in await page.GuestCards.AllAsync())
        {
            var card = new ParticipantCardComponent(root);
            var quantities = new Dictionary<string, int>(StringComparer.Ordinal);
            foreach (var item in await card.Items.AllAsync())
            {
                var name = (await ParticipantCardComponent.ItemName(item).InnerTextAsync()).Trim();
                quantities[name] = Money.ParseCount(await ParticipantCardComponent.ItemQuantity(item).InnerTextAsync());
            }

            cards.Add(new ParticipantCardView(
                (await card.Title.InnerTextAsync()).Trim(),
                (await card.Status.InnerTextAsync()).Trim(),
                quantities,
                Money.Parse(await card.Subtotal.InnerTextAsync()),
                await card.NoDishes.IsVisibleAsync()));
        }

        return cards;
    }
}
