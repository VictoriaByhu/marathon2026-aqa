using Restaurant_Aqa_Framework.Screenplay.GroupOrders;

namespace Restaurant_Aqa_Framework.Screenplay.Questions.GroupOrders;

public sealed class TheExpectedParticipants(string booking = GroupNames.Main) : IQuestion<IReadOnlyList<ExpectedParticipant>>
{
    public Task<IReadOnlyList<ExpectedParticipant>> AnsweredByAsync(Actor actor)
    {
        var menu = actor.Recall<GroupMenu>();
        var expected = new List<ExpectedParticipant>();

        if (actor.TryRecall<OwnerPlace>(out var place) && string.Equals(place.Booking, booking, StringComparison.Ordinal))
            expected.Add(new ExpectedParticipant(
                actor.TheOwner().FirstName, "Owner", new Dictionary<string, int>(), 0m, false));

        expected.AddRange(actor.TheCartsIn(booking).Select(cart => new ExpectedParticipant(
            cart.GuestName, "Guest", cart.Items, menu.PriceOf(cart.Items), cart.IsSubmitted)));

        return Task.FromResult<IReadOnlyList<ExpectedParticipant>>(expected);
    }
}
