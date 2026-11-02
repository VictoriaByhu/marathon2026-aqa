using Restaurant_Aqa_Framework.Api.Dtos.GroupOrders;

namespace Restaurant_Aqa_Framework.Screenplay.Questions.GroupOrders;

public sealed class TheRemindersSent : IQuestion<PendingRemindersResponse>
{
    public Task<PendingRemindersResponse> AnsweredByAsync(Actor actor) =>
        Task.FromResult(actor.TheLastResponse().Json<PendingRemindersResponse>());
}
