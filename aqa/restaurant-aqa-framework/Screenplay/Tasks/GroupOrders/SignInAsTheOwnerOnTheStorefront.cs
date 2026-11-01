using Restaurant_Aqa_Framework.Screenplay.GroupOrders;

namespace Restaurant_Aqa_Framework.Screenplay.Tasks.GroupOrders;

public sealed class SignInAsTheOwnerOnTheStorefront : ITask
{
    public Task PerformAsAsync(Actor actor)
    {
        var owner = actor.TheOwner();
        return actor.AttemptsToAsync(new SignInThroughTheLoginForm(owner.Email, owner.Password));
    }
}
