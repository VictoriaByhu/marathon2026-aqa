using Restaurant_Aqa_Framework.Screenplay;
using Restaurant_Aqa_Framework.Screenplay.GroupOrders;
using Restaurant_Aqa_Framework.Screenplay.Questions.GroupOrders;
using Restaurant_Aqa_Framework.Screenplay.Tasks.GroupOrders;

namespace Restaurant_Aqa_Framework.StepDefinitions;

[Binding]
public sealed class GroupPreOrderRemovalStepDefinitions(Actor actor) : StepDefinitionBase(actor)
{
    [When("the owner removes {string} from the group")]
    [When("the owner tries to remove {string} from the group")]
    public Task WhenTheOwnerRemoves(string guest) => Actor.AttemptsToAsync(RemoveFromTheGroup.TheGuest(guest));

    [When("{someone} tries to remove {string} from the group")]
    public Task WhenSomeoneTriesToRemove(Requester someone, string guest) =>
        Actor.AttemptsToAsync(RemoveFromTheGroup.TheGuest(guest, someone));

    [When("the owner tries to remove someone who never joined the group")]
    public Task WhenTheOwnerRemovesSomeoneWhoNeverJoined() =>
        Actor.AttemptsToAsync(RemoveFromTheGroup.SomeoneWhoNeverJoined());

    [When("the owner tries to remove their own place from the group")]
    public Task WhenTheOwnerRemovesTheirOwnPlace() => Actor.AttemptsToAsync(RemoveFromTheGroup.TheOwnersPlace());

    [When("the owner removes {string} on the page")]
    public Task WhenTheOwnerRemovesOnThePage(string guest) => Actor.AttemptsToAsync(new RemoveTheGuestOnThePage(guest));

    [Then("{string} is no longer in the group")]
    public async Task ThenGuestIsGone(string guest)
    {
        var outcome = Actor.TheLastResponse();
        await Assert.That(outcome.Succeeded).IsTrue().Because($"the removal answered {outcome.Status}: {outcome.Body}");

        var shown = await Actor.AsksForAsync(new TheParticipantsShown());
        await Assert.That(shown.Participants.Any(participant => participant.Name == guest)).IsFalse()
            .Because($"the group as returned was: {outcome.Body}");
    }

    [Then("{string} is still in the group")]
    public Task ThenGuestIsStillInTheGroup(string guest) => AssertStillInAsync(guest, GroupNames.Main);

    [Then("{string} is still in the second group")]
    public Task ThenGuestIsStillInTheSecondGroup(string guest) => AssertStillInAsync(guest, GroupNames.Second);

    private async Task AssertStillInAsync(string guest, string booking)
    {
        await Actor.AttemptsToAsync(new ViewTheGroup(Requester.Owner, Actor.TheBooking(booking).GroupReservationId));
        var shown = await Actor.AsksForAsync(new TheParticipantsShown());

        await Assert.That(shown.Participants.Any(participant => participant.Name == guest)).IsTrue()
            .Because($"the group as returned was: {Actor.TheLastResponse().Body}");
    }
}
