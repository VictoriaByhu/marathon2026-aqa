using Microsoft.Playwright;
using Restaurant_Aqa_Framework.Ui;

namespace Restaurant_Aqa_Framework.Screenplay.Questions.GroupOrders;

public sealed record GroupPageState(string Badge, bool SentNoteShown, bool CanSubmit, bool SaysNothingAddedYet);

public sealed class TheGroupPageState(bool waitForTheSentNote = false) : IQuestion<GroupPageState>
{
    public async Task<GroupPageState> AnsweredByAsync(Actor actor)
    {
        var page = new GroupPreOrderPage(actor.LookingAtThePage());
        await page.StatusBadge.WaitForAsync(new() { State = WaitForSelectorState.Visible });

        if (waitForTheSentNote)
            await page.SentNote.BecomesVisibleAsync();

        return new GroupPageState(
            (await page.StatusBadge.InnerTextAsync()).Trim(),
            await page.SentNote.IsVisibleAsync(),
            await page.SubmitButton.IsVisibleAsync() && await page.SubmitButton.IsEnabledAsync(),
            await page.NothingAddedYet.IsVisibleAsync());
    }
}
