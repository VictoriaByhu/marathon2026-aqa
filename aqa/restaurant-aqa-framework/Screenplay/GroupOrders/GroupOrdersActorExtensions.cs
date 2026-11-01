using Restaurant_Aqa_Framework.Api.Seeding;

namespace Restaurant_Aqa_Framework.Screenplay.GroupOrders;

public static class GroupOrdersActorExtensions
{
    public const string OwnerKey = "owner";

    public static CustomerAccount TheOwner(this Actor actor) => actor.Recall<CustomerAccount>(OwnerKey);

    public static GroupBooking TheBooking(this Actor actor, string booking = GroupNames.Main) =>
        actor.Recall<GroupBooking>(booking);

    public static GuestCart TheCartOf(this Actor actor, string guestName) => actor.Recall<GuestCart>(guestName);

    public static IReadOnlyList<GuestCart> TheCartsIn(this Actor actor, string booking = GroupNames.Main) =>
        actor.RecallAll<GuestCart>()
            .Where(cart => string.Equals(cart.Booking, booking, StringComparison.Ordinal) && !cart.IsRemoved)
            .ToList();

    public static CleanupScope TheCleanup(this Actor actor) => actor.Recall<CleanupScope>();
}
