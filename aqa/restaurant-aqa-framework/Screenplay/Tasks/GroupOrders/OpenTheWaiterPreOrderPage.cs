using Restaurant_Aqa_Framework.Configuration;
using Restaurant_Aqa_Framework.Screenplay.GroupOrders;
using Restaurant_Aqa_Framework.Ui;

namespace Restaurant_Aqa_Framework.Screenplay.Tasks.GroupOrders;

public sealed class OpenTheWaiterPreOrderPage(string booking = GroupNames.Main) : ITask
{
    public async Task PerformAsAsync(Actor actor)
    {
        var browser = actor.LookingAtThePage();
        var (username, password) = Settings.Current.RequireWaiter();

        await new StorefrontSession(browser).SignOutAsync();
        await actor.AttemptsToAsync(new SignInThroughTheLoginForm(username, password));
        await new WaiterPreOrderPage(browser).OpenForAsync(actor.TheBooking(booking).ReservationId, actor.TheOwner().FirstName);
    }
}
