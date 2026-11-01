using Restaurant_Aqa_Framework.Api;
using Restaurant_Aqa_Framework.Screenplay.GroupOrders;
using Restaurant_Aqa_Framework.Screenplay.Questions.GroupOrders;

namespace Restaurant_Aqa_Framework.Screenplay.Tasks.GroupOrders;

public sealed class RemoveFromTheGroup : ITask
{
    private readonly Func<Actor, string> _sessionOf;
    private readonly Requester _requester;
    private readonly string _booking;
    private readonly string? _guestName;

    private RemoveFromTheGroup(Func<Actor, string> sessionOf, Requester requester, string booking, string? guestName)
    {
        _sessionOf = sessionOf;
        _requester = requester;
        _booking = booking;
        _guestName = guestName;
    }

    public static RemoveFromTheGroup TheGuest(string guestName, Requester requester = Requester.Owner, string booking = GroupNames.Main) =>
        new(actor => actor.TheCartOf(guestName).SessionId, requester, booking, guestName);

    public static RemoveFromTheGroup TheOwnersPlace() =>
        new(actor => actor.Recall<OwnerPlace>().SessionId, Requester.Owner, GroupNames.Main, null);

    public static RemoveFromTheGroup SomeoneWhoNeverJoined() =>
        new(_ => Guid.NewGuid().ToString(), Requester.Owner, GroupNames.Main, null);

    public async Task PerformAsAsync(Actor actor)
    {
        var token = await actor.AsksForAsync(new TheCredentialsOf(_requester));
        var response = actor.TheLastResponse();

        await response.CaptureAsync(await new GroupOrdersApi(actor.UsingTheApi()).RemoveParticipantAsync(
            actor.TheBooking(_booking).GroupReservationId, _sessionOf(actor), token));

        if (response.Succeeded && _guestName is not null)
            actor.TheCartOf(_guestName).MarkRemoved();
    }
}
