using Restaurant_Aqa_Framework.Ui;

namespace Restaurant_Aqa_Framework.Screenplay.Questions.GroupOrders;

public sealed class TheGrandTotalOnThePage : IQuestion<decimal>
{
    public async Task<decimal> AnsweredByAsync(Actor actor) =>
        Money.Parse(await new GroupPreOrderPage(actor.LookingAtThePage()).GrandTotal.InnerTextAsync());
}
