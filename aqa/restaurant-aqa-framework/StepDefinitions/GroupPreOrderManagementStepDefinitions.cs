using Restaurant_Aqa_Framework.Screenplay;
using Restaurant_Aqa_Framework.Screenplay.GroupOrders;
using Restaurant_Aqa_Framework.Screenplay.Questions;
using Restaurant_Aqa_Framework.Screenplay.Questions.GroupOrders;
using Restaurant_Aqa_Framework.Screenplay.Tasks.GroupOrders;

namespace Restaurant_Aqa_Framework.StepDefinitions;

[Binding]
public sealed class GroupPreOrderManagementStepDefinitions(Actor actor) : StepDefinitionBase(actor)
{
    private static readonly Dictionary<string, string> ActivityTypes = new(StringComparer.Ordinal)
    {
        ["joined"] = "GuestJoined",
        ["added a dish"] = "DishAdded",
        ["removed a dish"] = "DishRemoved",
        ["submitted the cart"] = "CartSubmitted",
        ["removed by owner"] = "GuestRemoved",
        ["order edited by owner"] = "OrderEdited",
        ["reminded"] = "ReminderSent",
        ["group submitted"] = "GroupSubmitted",
    };

    [When("the owner changes {string}'s order to:")]
    public async Task WhenTheOwnerChangesTheOrder(string guest, DataTable dishes)
    {
        await Actor.AttemptsToAsync(ChangeTheGuestsOrder.To(guest, dishes.ToDishQuantities()));
        await AssertSucceededAsync("changing the order");
    }

    [When("the owner tries to empty {string}'s cart")]
    public Task WhenTheOwnerTriesToEmptyTheCart(string guest) => Actor.AttemptsToAsync(ChangeTheGuestsOrder.ToNothing(guest));

    [When("the owner tries to add a dish from another restaurant to {string}'s order")]
    public Task WhenTheOwnerAddsAForeignDish(string guest) =>
        Actor.AttemptsToAsync(ChangeTheGuestsOrder.WithADishFromAnotherRestaurant(guest));

    [When("{someone} tries to change {string}'s order")]
    public Task WhenSomeoneTriesToChangeTheOrder(Requester someone, string guest)
    {
        var items = new Dictionary<string, int>(StringComparer.Ordinal) { ["B"] = 2 };
        return Actor.AttemptsToAsync(ChangeTheGuestsOrder.To(guest, items, someone));
    }

    [When("the owner opens the menu of the group's restaurant")]
    public Task WhenTheOwnerOpensTheMenu() => Actor.AttemptsToAsync(new ReadTheGroupMenu());

    [When("the owner reminds {string}")]
    public Task WhenTheOwnerReminds(string guest) => Actor.AttemptsToAsync(new RemindTheGuest(guest));

    [When("{someone} tries to remind {string}")]
    public Task WhenSomeoneTriesToRemind(Requester someone, string guest) => Actor.AttemptsToAsync(new RemindTheGuest(guest, someone));

    [When("the owner reminds everyone who is still choosing")]
    public Task WhenTheOwnerRemindsEveryone() => Actor.AttemptsToAsync(new RemindEveryoneStillChoosing());

    [Given("the owner has removed {string} from the group")]
    public async Task GivenTheOwnerHasRemoved(string guest)
    {
        await Actor.AttemptsToAsync(RemoveFromTheGroup.TheGuest(guest));
        await AssertSucceededAsync("removing the guest");
    }

    [When("the owner looks at the group's activity")]
    public Task WhenTheOwnerLooksAtTheActivity() => Actor.AttemptsToAsync(new ReadTheGroupActivity());

    [When("{someone} tries to look at the group's activity")]
    public Task WhenSomeoneLooksAtTheActivity(Requester someone) => Actor.AttemptsToAsync(new ReadTheGroupActivity(someone));

    [When("the owner opens the group pre-order review page")]
    public Task WhenTheOwnerOpensTheReviewPage() => Actor.AttemptsToAsync(new OpenTheGroupReviewPage());

    [When("the owner changes {string}'s order on the page to:")]
    public Task WhenTheOwnerChangesTheOrderOnThePage(string guest, DataTable dishes) =>
        Actor.AttemptsToAsync(new ChangeTheGuestsOrderOnThePage(guest, dishes.ToDishQuantities()));

    [When("the owner sends {string} a reminder on the page")]
    public Task WhenTheOwnerRemindsOnThePage(string guest) => Actor.AttemptsToAsync(new RemindTheGuestOnThePage(guest));

    [When("the owner opens the recent activity")]
    public Task WhenTheOwnerOpensTheRecentActivity() => Actor.AttemptsToAsync(new OpenTheRecentActivity());

    [When("{string} opens the guest pre-order page")]
    public Task WhenTheGuestOpensTheirPage(string guest) => Actor.AttemptsToAsync(new OpenTheGuestsPreOrderPage(guest));

    [When("the owner chooses the group pre-order on the reservation card")]
    public Task WhenTheOwnerChoosesTheGroupPreOrder() => Actor.AttemptsToAsync(new ChooseTheGroupPreOrderOfTheReservation());

    [Then("{string} sees the same order in the cart")]
    [Then("{string} still has the same order in the cart")]
    public async Task ThenTheGuestSeesTheSameOrder(string guest)
    {
        var view = await Actor.AsksForAsync(new TheCartAsSeenByTheGuest(guest));
        var menu = Recall<GroupMenu>();
        var seen = view.Items.ToDictionary(item => menu.LabelOf(item.Dish.Id), item => item.Quantity, StringComparer.Ordinal);

        await Assert.That(seen.Describe()).IsEqualTo(Actor.TheCartOf(guest).Items.Describe());
    }

    [Then("the owner is told that the dish is not on the restaurant's menu")]
    public async Task ThenTheDishIsNotOnTheMenu()
    {
        var outcome = await Actor.AsksForAsync(new TheOutcome());

        await Assert.That(outcome.Status).IsEqualTo(400).Because($"the response body was: {outcome.Body}");
        await Assert.That(outcome.ErrorMessage).Contains("is not on the menu of this restaurant");
    }

    [Then("the menu offers the dishes the guests can order")]
    public async Task ThenTheMenuOffersTheDishes()
    {
        var offered = (await Actor.AsksForAsync(new TheMenuOffered())).Select(dish => dish.Id).ToHashSet(StringComparer.Ordinal);

        foreach (var dish in Recall<GroupMenu>().DishesByLabel.Values)
            await Assert.That(offered.Contains(dish.Id)).IsTrue().Because($"{dish.Name} is missing from the menu: {Actor.TheLastResponse().Body}");
    }

    [Then("{string} is reminded to submit the cart")]
    public async Task ThenTheGuestIsReminded(string guest)
    {
        await AssertSucceededAsync("the reminder");
        var view = await Actor.AsksForAsync(new TheCartAsSeenByTheGuest(guest));
        await Assert.That(view.RemindedAt.HasValue).IsTrue();
    }

    [Then("{string} is not reminded")]
    public async Task ThenTheGuestIsNotReminded(string guest)
    {
        var view = await Actor.AsksForAsync(new TheCartAsSeenByTheGuest(guest));
        await Assert.That(view.RemindedAt.HasValue).IsFalse();
    }

    [Then("the reminder goes to:")]
    public async Task ThenTheReminderGoesTo(DataTable guests)
    {
        await AssertSucceededAsync("the reminders");
        var reminded = (await Actor.AsksForAsync(new TheRemindersSent())).Reminded.Select(reminder => reminder.GuestName).Order(StringComparer.Ordinal);
        var expected = guests.Rows.Select(row => row["guest"]).Order(StringComparer.Ordinal);

        await Assert.That(string.Join(", ", reminded)).IsEqualTo(string.Join(", ", expected));
        foreach (var row in guests.Rows)
            await Assert.That((await Actor.AsksForAsync(new TheCartAsSeenByTheGuest(row["guest"]))).RemindedAt.HasValue).IsTrue();
    }

    [Then("the activity reads, newest first:")]
    public async Task ThenTheActivityReads(DataTable events)
    {
        await AssertSucceededAsync("reading the activity");
        var shown = await Actor.AsksForAsync(new TheActivityShown());

        var actual = string.Join(Environment.NewLine, shown.Select(entry => $"{entry.Type} {entry.GuestName}"));
        var expected = string.Join(Environment.NewLine, events.Rows.Select(row => $"{ActivityTypes[row["event"]]} {row["guest"]}"));

        await Assert.That(actual).IsEqualTo(expected);
    }

    [Then("the review page shows {string} with:")]
    public async Task ThenTheReviewPageShowsTheGuest(string guest, DataTable dishes)
    {
        var menu = Recall<GroupMenu>();
        var expected = dishes.ToDishQuantities()
            .ToDictionary(item => menu.Dish(item.Key).Name, item => item.Value, StringComparer.Ordinal);
        var shown = await Actor.AsksForAsync(new TheGuestOnTheReviewPage(guest, expected.Count));

        await Assert.That(shown.QuantitiesByDish.Describe()).IsEqualTo(expected.Describe());
    }

    [Then("the review page shows that {string} was reminded")]
    public async Task ThenTheReviewPageShowsTheReminder(string guest)
    {
        var shown = await Actor.AsksForAsync(new TheGuestOnTheReviewPage(guest, Actor.TheCartOf(guest).Items.Count));
        await Assert.That(shown.ShowsReminder).IsTrue();
    }

    [Then("{string} sees the host's reminder")]
    public async Task ThenTheGuestSeesTheReminder(string guest) =>
        await Assert.That(await Actor.AsksForAsync(new TheGuestSeesTheReminder())).IsTrue().Because($"{guest}'s page shows no reminder");

    [Then("the recent activity mentions {string}")]
    public async Task ThenTheRecentActivityMentions(string entry)
    {
        var shown = await Actor.AsksForAsync(new TheRecentActivityOnThePage(entry));
        await Assert.That(shown.Contains(entry)).IsTrue().Because($"the activity shows: {string.Join(" | ", shown)}");
    }

    [Then("the group pre-order review page of the reservation is shown")]
    public async Task ThenTheReviewPageIsShown()
    {
        var expectedPath = $"/reservations/{Actor.TheBooking().ReservationId}/group-order/review";
        var address = await Actor.AsksForAsync(new TheCurrentAddress(expectedPath));

        await Assert.That(address).Contains(expectedPath);
    }

    private async Task AssertSucceededAsync(string what)
    {
        var outcome = await Actor.AsksForAsync(new TheOutcome());
        await Assert.That(outcome.Succeeded).IsTrue().Because($"{what} answered {outcome.Status}: {outcome.Body}");
    }
}
