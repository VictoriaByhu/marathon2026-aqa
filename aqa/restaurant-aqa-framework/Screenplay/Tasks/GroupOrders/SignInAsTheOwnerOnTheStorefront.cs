using Restaurant_Aqa_Framework.Screenplay.GroupOrders;

namespace Restaurant_Aqa_Framework.Screenplay.Tasks.GroupOrders;

public sealed class SignInAsTheOwnerOnTheStorefront : ITask
{
    public async Task PerformAsAsync(Actor actor)
    {
        var owner = actor.TheOwner();
        await actor.AttemptsToAsync(new SignInThroughTheLoginForm(owner.Email, owner.Password));
        await actor.LookingAtThePage().WaitForFunctionAsync("() => localStorage.getItem('user') !== null");
    }
}
