using Restaurant_Aqa_Framework.Api;
using Restaurant_Aqa_Framework.Api.Dtos.Auth;
using Restaurant_Aqa_Framework.Api.Seeding;
using Restaurant_Aqa_Framework.Screenplay.GroupOrders;

namespace Restaurant_Aqa_Framework.Screenplay.Tasks.GroupOrders;

public sealed class SignUpACustomer(string role) : ITask
{
    public async Task PerformAsAsync(Actor actor)
    {
        var response = actor.TheLastResponse();
        var auth = new AuthApi(actor.UsingTheApi());
        var details = SampleDataFactory.RegisterRequest();

        await response.ExpectSuccessAsync(await auth.RegisterAsync(details), "Registering a customer");
        var session = await response.ExpectSuccessAsync<SignInResponse>(
            await auth.SignInAsync(new SignInRequest(details.Email, details.Password)),
            "Signing the customer in");

        actor.Remember(role, new CustomerAccount(details.Email, details.Password, details.FirstName, session.IdToken));
    }
}
