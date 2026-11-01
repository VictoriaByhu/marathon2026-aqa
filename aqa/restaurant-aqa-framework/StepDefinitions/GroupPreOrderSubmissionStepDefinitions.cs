using Restaurant_Aqa_Framework.Screenplay;
using Restaurant_Aqa_Framework.Screenplay.GroupOrders;
using Restaurant_Aqa_Framework.Screenplay.Questions.GroupOrders;
using Restaurant_Aqa_Framework.Screenplay.Tasks.GroupOrders;

namespace Restaurant_Aqa_Framework.StepDefinitions;

[Binding]
public sealed class GroupPreOrderSubmissionStepDefinitions(Actor actor) : StepDefinitionBase(actor)
{
    [When("the owner submits the group pre-order")]
    public Task WhenTheOwnerSubmits() => Actor.AttemptsToAsync(new SubmitTheGroupPreOrder());

    [When("{someone} tries to submit the group pre-order")]
    public Task WhenSomeoneTriesToSubmit(Requester someone) => Actor.AttemptsToAsync(new SubmitTheGroupPreOrder(someone));

    [When("the owner submits the pre-order of a group that does not exist")]
    public Task WhenTheOwnerSubmitsAMissingGroup() => Actor.AttemptsToAsync(SubmitTheGroupPreOrder.ForAGroupThatDoesNotExist());

    [When("the owner submits the group pre-order for a reservation that does not exist")]
    public Task WhenTheOwnerSubmitsForAMissingReservation() =>
        Actor.AttemptsToAsync(SubmitTheGroupPreOrder.ForAReservationThatDoesNotExist());

    [When("the owner submits the pre-order on the page")]
    public Task WhenTheOwnerSubmitsOnThePage() => Actor.AttemptsToAsync(new SubmitThePreOrderOnThePage());

    [When("the waiter opens the pre-order of the reservation")]
    public Task WhenTheWaiterOpensThePreOrder() => Actor.AttemptsToAsync(new OpenTheWaiterPreOrderPage());

    [Then("the pre-order is sent to the restaurant")]
    public async Task ThenThePreOrderIsSent()
    {
        var outcome = Actor.TheLastResponse();
        await Assert.That(outcome.Succeeded).IsTrue().Because($"the submission answered {outcome.Status}: {outcome.Body}");

        var order = await Actor.AsksForAsync(new TheOrderAsSeenBy(Requester.Owner));
        await Assert.That(order.Status).IsEqualTo("SubmittedByCustomer");
    }

    [Then("the restaurant receives:")]
    public async Task ThenTheRestaurantReceives(DataTable dishes)
    {
        var order = await Actor.AsksForAsync(new TheOrderAsSeenBy(Requester.Owner));

        await Assert.That(order.Items.Describe()).IsEqualTo(dishes.ToDishQuantities().Describe());
        await Assert.That(order.Status).IsEqualTo("SubmittedByCustomer");
    }

    [Then("the group is marked as submitted")]
    public async Task ThenTheGroupIsSubmitted() =>
        await Assert.That(await Actor.AsksForAsync(new TheGroupStatus())).IsEqualTo("Submitted");

    [Then("the group is still collecting pre-orders")]
    public async Task ThenTheGroupIsStillOpen() =>
        await Assert.That(await Actor.AsksForAsync(new TheGroupStatus())).IsEqualTo("Open");

    [Then("the waiter sees the order with:")]
    [Scope(Tag = "api")]
    public async Task ThenTheWaiterSeesTheOrder(DataTable dishes)
    {
        var order = await Actor.AsksForAsync(new TheOrderAsSeenBy(Requester.Waiter));

        await Assert.That(order.Items.Describe()).IsEqualTo(dishes.ToDishQuantities().Describe());
        await Assert.That(order.Status).IsEqualTo("SubmittedByCustomer");
    }

    [Then("the waiter sees the order with:")]
    [Scope(Tag = "ui")]
    public async Task ThenTheWaiterSeesTheOrderOnThePage(DataTable dishes)
    {
        var menu = Recall<GroupMenu>();
        var expected = dishes.ToDishQuantities()
            .ToDictionary(item => menu.Dish(item.Key).Name, item => item.Value, StringComparer.Ordinal);
        var shown = await Actor.AsksForAsync(new TheWaiterPreOrderLines(expected.Count));

        await Assert.That(shown.Describe()).IsEqualTo(expected.Describe());
    }

    [Then("the page shows that the pre-order was sent to the restaurant")]
    public async Task ThenThePageShowsThePreOrderWasSent()
    {
        var state = await Actor.AsksForAsync(new TheGroupPageState(waitForTheSentNote: true));

        await Assert.That(state.SentNoteShown).IsTrue();
        await Assert.That(state.Badge).IsEqualTo("Sent to Restaurant");
        await Assert.That(state.CanSubmit).IsFalse();
    }

    [Then("the owner cannot submit the pre-order on the page")]
    public async Task ThenTheOwnerCannotSubmitOnThePage()
    {
        var state = await Actor.AsksForAsync(new TheGroupPageState());

        await Assert.That(state.CanSubmit).IsFalse();
        await Assert.That(state.Badge).IsEqualTo("Collecting pre-orders");
    }
}
