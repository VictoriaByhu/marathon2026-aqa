using Microsoft.Playwright;
using Restaurant_Aqa_Framework.Api.Dtos.GroupOrders;

namespace Restaurant_Aqa_Framework.Api;

public sealed class GuestSessionsApi(IAPIRequestContext api) : ApiClientBase(api)
{
    public Task<IAPIResponse> JoinAsync(string groupReservationId, string guestName) =>
        Api.PostAsync(Url("/guest/sessions"), new APIRequestContextOptions
        {
            DataString = Serialize(new CreateGuestSessionRequest(groupReservationId, guestName)),
            Headers = Headers(),
        });

    public Task<IAPIResponse> GetSessionAsync(string guestSessionId, string sessionToken) =>
        GetAsync(Url($"/guest/sessions/{guestSessionId}"), new APIRequestContextOptions
        {
            Headers = Headers(sessionToken),
        });

    public Task<IAPIResponse> AddDishAsync(string guestSessionId, string sessionToken, string dishId, int quantity) =>
        Api.PostAsync(Url($"/guest/sessions/{guestSessionId}/dishes"), new APIRequestContextOptions
        {
            DataString = Serialize(new GuestDishRequest(dishId, quantity)),
            Headers = Headers(sessionToken),
        });

    public Task<IAPIResponse> RemoveDishAsync(string guestSessionId, string sessionToken, string dishId, int quantity) =>
        Api.DeleteAsync(Url($"/guest/sessions/{guestSessionId}/dishes"), new APIRequestContextOptions
        {
            DataString = Serialize(new GuestDishRequest(dishId, quantity)),
            Headers = Headers(sessionToken),
        });

    public Task<IAPIResponse> SubmitCartAsync(string guestSessionId, string sessionToken) =>
        Api.PostAsync(Url($"/guest/sessions/{guestSessionId}/submit"), new APIRequestContextOptions
        {
            Headers = Headers(sessionToken),
        });
}
