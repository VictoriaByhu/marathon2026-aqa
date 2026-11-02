using Restaurant_Aqa_Framework.Ui;

namespace Restaurant_Aqa_Framework.Screenplay.Tasks.GroupOrders;

public sealed class ChooseTheGroupPreOrderOfTheReservation : ITask
{
    public async Task PerformAsAsync(Actor actor)
    {
        await actor.AttemptsToAsync(new SignInAsTheOwnerOnTheStorefront());

        var page = new ReservationsListPage(actor.LookingAtThePage());
        await page.NavigateAsync();
        await page.GroupPreOrderButton.ClickAsync();
    }
}
