using System.Globalization;
using Restaurant_Aqa_Framework.Api.Dtos.GroupOrders;
using Restaurant_Aqa_Framework.Screenplay;
using Restaurant_Aqa_Framework.Screenplay.GroupOrders;
using Restaurant_Aqa_Framework.Screenplay.Questions.GroupOrders;
using Restaurant_Aqa_Framework.Screenplay.Tasks.GroupOrders;

namespace Restaurant_Aqa_Framework.StepDefinitions;

[Binding]
public sealed class GroupPreOrderParticipantStepDefinitions(Actor actor) : StepDefinitionBase(actor)
{
    [When("the owner looks at the group pre-order")]
    public Task WhenTheOwnerLooks() => Actor.AttemptsToAsync(new ViewTheGroup());

    [When("{someone} asks to see the participants of the group")]
    public Task WhenSomeoneAsksToSee(Requester someone) => Actor.AttemptsToAsync(new ViewTheGroup(someone));

    [When("the owner asks to see the participants of a group that does not exist")]
    public Task WhenTheOwnerAsksForAMissingGroup() => Actor.AttemptsToAsync(ViewTheGroup.ThatDoesNotExist());

    [When("the owner opens the group pre-order page")]
    public Task WhenTheOwnerOpensThePage() =>
        Actor.AttemptsToAsync(new SignInAsTheOwnerOnTheStorefront(), new OpenTheGroupPreOrderPage());

    [Then("the owner sees every participant with their dishes, quantities and subtotal")]
    [Scope(Tag = "api")]
    public async Task ThenTheOwnerSeesEveryParticipant()
    {
        var shown = await Actor.AsksForAsync(new TheParticipantsShown());
        var expected = await Actor.AsksForAsync(new TheExpectedParticipants());
        var menu = Recall<GroupMenu>();

        var actualLines = shown.Participants.Select(participant => Line(
            participant.Type,
            participant.Name,
            participant.Items.ToDictionary(item => menu.LabelOf(item.Dish.Id), item => item.Quantity, StringComparer.Ordinal),
            participant.Total,
            participant.IsSubmitted));
        var expectedLines = expected.Select(participant => Line(
            participant.Type, participant.Name, participant.Items, participant.Subtotal, participant.IsSubmitted));

        await Assert.That(Sorted(actualLines)).IsEqualTo(Sorted(expectedLines))
            .Because($"the group as returned was: {Actor.TheLastResponse().Body}");
    }

    [Then("the owner sees every participant with their dishes, quantities and subtotal")]
    [Scope(Tag = "ui")]
    public async Task ThenTheOwnerSeesEveryParticipantOnThePage()
    {
        var expected = (await Actor.AsksForAsync(new TheExpectedParticipants()))
            .Where(participant => participant.Type == "Guest")
            .ToList();
        var cards = await Actor.AsksForAsync(new TheGuestCardsOnThePage(expected.Count));
        var menu = Recall<GroupMenu>();

        var actualLines = cards.Select(card => Line(
            "Guest",
            card.Title["Guest: ".Length..],
            card.QuantitiesByDish,
            card.Subtotal,
            card.Status == "Submitted"));
        var expectedLines = expected.Select(participant => Line(
            "Guest",
            participant.Name,
            participant.Items.ToDictionary(item => menu.Dish(item.Key).Name, item => item.Value, StringComparer.Ordinal),
            participant.Subtotal,
            participant.IsSubmitted));

        await Assert.That(Sorted(actualLines)).IsEqualTo(Sorted(expectedLines));
    }

    [Then("the overall total is the sum of the participants' subtotals")]
    public async Task ThenTheOverallTotalIsTheSum()
    {
        var shown = await Actor.AsksForAsync(new TheParticipantsShown());
        var expected = await Actor.AsksForAsync(new TheExpectedParticipants());

        await Assert.That(shown.Total).IsEqualTo(shown.Participants.Sum(participant => participant.Total))
            .Because($"the group as returned was: {Actor.TheLastResponse().Body}");
        await Assert.That(shown.Total).IsEqualTo(expected.Sum(participant => participant.Subtotal));
    }

    [Then("the overall total has grown by the price of {int} of dish {string}")]
    public Task ThenTheTotalHasGrown(int quantity, string dish) => AssertTotalChangedAsync(quantity, dish);

    [Then("the overall total has dropped by the price of {int} of dish {string}")]
    public Task ThenTheTotalHasDropped(int quantity, string dish) => AssertTotalChangedAsync(-quantity, dish);

    [Then("{string} is listed as still choosing")]
    [Scope(Tag = "api")]
    public async Task ThenGuestIsStillChoosing(string guest)
    {
        var participant = await TheParticipantNamedAsync(guest);
        await Assert.That(participant.IsSubmitted).IsFalse();
        await Assert.That(participant.Items.Count).IsGreaterThan(0);
    }

    [Then("{string} is listed as still choosing")]
    [Scope(Tag = "ui")]
    public async Task ThenGuestIsStillChoosingOnThePage(string guest)
    {
        var card = await TheCardOfAsync(guest);
        await Assert.That(card.Status).IsEqualTo("Still choosing");
        await Assert.That(card.QuantitiesByDish.Count).IsGreaterThan(0);
    }

    [Then("{string} is listed without any dishes")]
    [Scope(Tag = "api")]
    public async Task ThenGuestHasNoDishes(string guest)
    {
        var participant = await TheParticipantNamedAsync(guest);
        await Assert.That(participant.Items.Count).IsEqualTo(0);
        await Assert.That(participant.Total).IsEqualTo(0m);
        await Assert.That(participant.IsSubmitted).IsFalse();
    }

    [Then("{string} is listed without any dishes")]
    [Scope(Tag = "ui")]
    public async Task ThenGuestHasNoDishesOnThePage(string guest)
    {
        var card = await TheCardOfAsync(guest);
        await Assert.That(card.ShowsNoDishes).IsTrue();
        await Assert.That(card.Subtotal).IsEqualTo(0m);
    }

    [Then("the group has no participants and nothing to pay")]
    public async Task ThenTheGroupIsEmpty()
    {
        var shown = await Actor.AsksForAsync(new TheParticipantsShown());
        await Assert.That(shown.Participants.Count).IsEqualTo(0).Because($"the group as returned was: {Actor.TheLastResponse().Body}");
        await Assert.That(shown.Total).IsEqualTo(0m);
    }

    [Then("the owner is told that the participants have not added any dishes yet")]
    public async Task ThenTheOwnerIsToldNothingWasAdded()
    {
        var state = await Actor.AsksForAsync(new TheGroupPageState());
        await Assert.That(state.SaysNothingAddedYet).IsTrue();
        await Assert.That(await Actor.AsksForAsync(new TheGrandTotalOnThePage())).IsEqualTo(0m);
    }

    [Then("the grand total covers only the submitted carts")]
    public async Task ThenTheGrandTotalCoversSubmittedCarts()
    {
        var expected = (await Actor.AsksForAsync(new TheExpectedParticipants()))
            .Where(participant => participant.IsSubmitted)
            .Sum(participant => participant.Subtotal);
        await Assert.That(await Actor.AsksForAsync(new TheGrandTotalOnThePage())).IsEqualTo(expected);
    }

    [Then("the owner is listed first as the host of the group")]
    public async Task ThenTheOwnerIsListedFirst()
    {
        var shown = await Actor.AsksForAsync(new TheParticipantsShown());
        var first = shown.Participants.FirstOrDefault();

        await Assert.That(first).IsNotNull().Because($"the group as returned was: {Actor.TheLastResponse().Body}");
        await Assert.That(first!.Type).IsEqualTo("Owner");
        await Assert.That(first.Name).IsEqualTo(Actor.TheOwner().FirstName);
        await Assert.That(first.Id).IsEqualTo(Recall<OwnerPlace>().SessionId);
    }

    private async Task AssertTotalChangedAsync(int quantity, string dish)
    {
        var shown = await Actor.AsksForAsync(new TheParticipantsShown());
        var expected = Recall<EarlierTotal>().Amount + (Recall<GroupMenu>().Dish(dish).Price * quantity);

        await Assert.That(shown.Total).IsEqualTo(expected).Because($"the group as returned was: {Actor.TheLastResponse().Body}");
    }

    private async Task<ParticipantResponse> TheParticipantNamedAsync(string guest)
    {
        var shown = await Actor.AsksForAsync(new TheParticipantsShown());
        return shown.Participants.SingleOrDefault(participant => participant.Name == guest)
            ?? throw new InvalidOperationException($"{guest} is not in the group: {Actor.TheLastResponse().Body}");
    }

    private async Task<ParticipantCardView> TheCardOfAsync(string guest)
    {
        var guests = (await Actor.AsksForAsync(new TheExpectedParticipants())).Count(participant => participant.Type == "Guest");
        var cards = await Actor.AsksForAsync(new TheGuestCardsOnThePage(guests));
        return cards.SingleOrDefault(card => card.Title == $"Guest: {guest}")
            ?? throw new InvalidOperationException($"The page shows no card for {guest}; cards: {string.Join(", ", cards.Select(card => card.Title))}");
    }

    private static string Line(string type, string name, IReadOnlyDictionary<string, int> items, decimal subtotal, bool submitted) =>
        $"{type} {name}: {items.Describe()} = {subtotal.ToString("0.00", CultureInfo.InvariantCulture)} ({(submitted ? "submitted" : "still choosing")})";

    private static string Sorted(IEnumerable<string> lines) =>
        string.Join(Environment.NewLine, lines.Order(StringComparer.Ordinal));
}
