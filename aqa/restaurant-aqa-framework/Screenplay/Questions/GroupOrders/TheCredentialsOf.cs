using Restaurant_Aqa_Framework.Screenplay.GroupOrders;
using Restaurant_Aqa_Framework.Screenplay.Tasks.GroupOrders;

namespace Restaurant_Aqa_Framework.Screenplay.Questions.GroupOrders;

public sealed class TheCredentialsOf(Requester requester, string booking = GroupNames.Main) : IQuestion<string?>
{
    public async Task<string?> AnsweredByAsync(Actor actor)
    {
        switch (requester)
        {
            case Requester.Owner:
                return actor.TheOwner().IdToken;
            case Requester.AnotherCustomer:
                if (!actor.TryRecall<OtherCustomer>(out var other))
                {
                    await actor.AttemptsToAsync(new SignUpACustomer(GroupNames.AnotherCustomer));
                    other = new OtherCustomer(actor.Recall<CustomerAccount>(GroupNames.AnotherCustomer));
                    actor.Remember(other);
                }

                return other.Account.IdToken;
            case Requester.Waiter:
                return (await actor.AsksForAsync(new TheWaiterSession())).IdToken;
            case Requester.Guest:
                return actor.TheCartsIn(booking).FirstOrDefault()?.SessionToken
                    ?? throw new InvalidOperationException($"Nobody has joined {booking} yet, so there is no guest to ask.");
            case Requester.Anonymous:
                return null;
            default:
                throw new ArgumentOutOfRangeException(nameof(requester), requester, null);
        }
    }
}
