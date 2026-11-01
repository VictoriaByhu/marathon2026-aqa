using Restaurant_Aqa_Framework.Ui;

namespace Restaurant_Aqa_Framework.Screenplay.Tasks.GroupOrders;

public sealed class SubmitThePreOrderOnThePage : ITask
{
    public Task PerformAsAsync(Actor actor) => new GroupPreOrderPage(actor.LookingAtThePage()).SubmitAsync();
}
