using Restaurant_Aqa_Framework.Api.Dtos.GroupOrders;

namespace Restaurant_Aqa_Framework.Screenplay.Questions.GroupOrders;

public sealed class TheActivityShown : IQuestion<IReadOnlyList<GroupActivityEntry>>
{
    public Task<IReadOnlyList<GroupActivityEntry>> AnsweredByAsync(Actor actor) =>
        Task.FromResult<IReadOnlyList<GroupActivityEntry>>(actor.TheLastResponse().Json<List<GroupActivityEntry>>());
}
