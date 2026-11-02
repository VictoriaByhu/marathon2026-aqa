using Restaurant_Aqa_Framework.Ui;

namespace Restaurant_Aqa_Framework.Screenplay.Questions.GroupOrders;

public sealed class TheGuestSeesTheReminder : IQuestion<bool>
{
    public Task<bool> AnsweredByAsync(Actor actor) =>
        new GuestPreOrderPage(actor.LookingAtThePage()).Reminder.BecomesVisibleAsync();
}
