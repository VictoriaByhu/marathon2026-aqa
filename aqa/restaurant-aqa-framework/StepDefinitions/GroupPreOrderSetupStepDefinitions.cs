using Restaurant_Aqa_Framework.Configuration;
using Restaurant_Aqa_Framework.Screenplay;
using Restaurant_Aqa_Framework.Screenplay.GroupOrders;
using Restaurant_Aqa_Framework.Screenplay.Questions.GroupOrders;
using Restaurant_Aqa_Framework.Screenplay.Tasks.GroupOrders;

namespace Restaurant_Aqa_Framework.StepDefinitions;

[Binding]
public sealed class GroupPreOrderSetupStepDefinitions(Actor actor) : StepDefinitionBase(actor)
{
    [Given("the restaurant is ready to take group pre-orders")]
    public Task GivenTheRestaurantIsReady()
    {
        Settings.Current.RequireWaiter();
        return Actor.AttemptsToAsync(new PrepareTheRestaurant());
    }

    [Given("a customer has booked a table for a group of {int}")]
    public Task GivenACustomerHasBookedATable(int guests) =>
        Actor.AttemptsToAsync(
            new SignUpACustomer(GroupOrdersActorExtensions.OwnerKey),
            new BookAGroupTable(guests));

    [Given("the customer has made a second group booking for {int} guests")]
    public Task GivenTheCustomerHasMadeASecondBooking(int guests) =>
        Actor.AttemptsToAsync(new BookAGroupTable(guests, GroupNames.Second));

    [Given("{string} has joined the group")]
    public Task GivenGuestHasJoined(string guest) => Actor.AttemptsToAsync(new JoinTheGroup(guest));

    [Given("{string} has joined the second group")]
    public Task GivenGuestHasJoinedTheSecondGroup(string guest) =>
        Actor.AttemptsToAsync(new JoinTheGroup(guest, GroupNames.Second));

    [Given("{string} has joined the group and chosen:")]
    public async Task GivenGuestHasJoinedAndChosen(string guest, DataTable dishes)
    {
        await Actor.AttemptsToAsync(new JoinTheGroup(guest));
        foreach (var (dish, quantity) in dishes.ToDishQuantities())
            await Actor.AttemptsToAsync(ChangeTheCart.Adding(guest, dish, quantity));
    }

    [Given("{string} has submitted the cart")]
    public Task GivenGuestHasSubmitted(string guest) => Actor.AttemptsToAsync(new SubmitTheCart(guest));

    [Given("the owner has pre-ordered for themselves:")]
    public Task GivenTheOwnerHasPreOrdered(DataTable dishes) =>
        Actor.AttemptsToAsync(new PreOrderForThemselves(dishes.ToDishQuantities()));

    [Given("the owner has taken their own place in the group")]
    [When("the owner takes their own place in the group")]
    public Task GivenTheOwnerHasTakenTheirPlace() => Actor.AttemptsToAsync(new OpenTheOwnersPlace());

    [Given("the owner has looked at the group pre-order")]
    public async Task GivenTheOwnerHasLooked()
    {
        await Actor.AttemptsToAsync(new ViewTheGroup());
        var group = await Actor.AsksForAsync(new TheParticipantsShown());
        Actor.Remember(new EarlierTotal(group.Total));
    }

    [Given("the owner has submitted the group pre-order")]
    public Task GivenTheOwnerHasSubmitted() => Actor.AttemptsToAsync(SubmitTheGroupPreOrder.ByTheOwner());

    [When("{string} adds {int} of dish {string}")]
    public Task WhenGuestAdds(string guest, int quantity, string dish) =>
        Actor.AttemptsToAsync(ChangeTheCart.Adding(guest, dish, quantity));

    [When("{string} takes {int} of dish {string} out of the cart")]
    public Task WhenGuestTakesOut(string guest, int quantity, string dish) =>
        Actor.AttemptsToAsync(ChangeTheCart.Removing(guest, dish, quantity));

    [When("{string} tries to submit the cart")]
    public Task WhenGuestTriesToSubmit(string guest) => Actor.AttemptsToAsync(new SubmitTheCart(guest, mustSucceed: false));
}
