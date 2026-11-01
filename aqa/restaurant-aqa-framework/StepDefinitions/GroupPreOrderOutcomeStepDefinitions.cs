using Restaurant_Aqa_Framework.Screenplay;
using Restaurant_Aqa_Framework.Screenplay.Questions;

namespace Restaurant_Aqa_Framework.StepDefinitions;

[Binding]
public sealed class GroupPreOrderOutcomeStepDefinitions(Actor actor) : StepDefinitionBase(actor)
{
    [Then("the request is refused because {reason}")]
    public async Task ThenTheRequestIsRefused(Refusal reason)
    {
        var outcome = await Actor.AsksForAsync(new TheOutcome());
        var expected = reason == Refusal.NotTheirGroup ? 403 : 401;

        await Assert.That(outcome.Status).IsEqualTo(expected).Because($"the response body was: {outcome.Body}");
    }

    [Then("the owner is told {string}")]
    [Then("the guest is told {string}")]
    public async Task ThenTheyAreTold(string message)
    {
        var outcome = await Actor.AsksForAsync(new TheOutcome());

        await Assert.That(outcome.Status).IsEqualTo(400).Because($"the response body was: {outcome.Body}");
        await Assert.That(outcome.ErrorMessage).IsEqualTo(message);
    }

    [Then("the owner is told that the group could not be found")]
    public Task ThenTheGroupIsNotFound() => AssertNotFoundAsync(404, "Group reservation was not found");

    [Then("the owner is told that the reservation could not be found")]
    public Task ThenTheReservationIsNotFound() => AssertNotFoundAsync(404, "was not found");

    [Then("the owner is told that the participant could not be found")]
    public Task ThenTheParticipantIsNotFound() => AssertNotFoundAsync(404, "not found");

    [Then("the owner is told that the order of the reservation could not be found")]
    public Task ThenTheOrderIsNotFound() => AssertNotFoundAsync(400, "Order for reservation");

    private async Task AssertNotFoundAsync(int status, string messagePart)
    {
        var outcome = await Actor.AsksForAsync(new TheOutcome());

        await Assert.That(outcome.Status).IsEqualTo(status).Because($"the response body was: {outcome.Body}");
        await Assert.That(outcome.ErrorMessage).Contains(messagePart);
    }
}
