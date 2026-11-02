using Restaurant_Aqa_Framework.Api.Dtos.GroupOrders;

namespace Restaurant_Aqa_Framework.Screenplay.Questions.GroupOrders;

public sealed class TheMenuOffered : IQuestion<IReadOnlyList<GroupMenuDishResponse>>
{
    public Task<IReadOnlyList<GroupMenuDishResponse>> AnsweredByAsync(Actor actor) =>
        Task.FromResult<IReadOnlyList<GroupMenuDishResponse>>(actor.TheLastResponse().Json<List<GroupMenuDishResponse>>());
}
