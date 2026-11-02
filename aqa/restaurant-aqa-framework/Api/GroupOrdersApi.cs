using Microsoft.Playwright;
using Restaurant_Aqa_Framework.Api.Dtos.GroupOrders;

namespace Restaurant_Aqa_Framework.Api;

public sealed class GroupOrdersApi(IAPIRequestContext api) : ApiClientBase(api)
{
    public Task<IAPIResponse> GetInviteTokenAsync(string reservationId, string? bearerToken) =>
        GetAsync(Url($"/reservations/{reservationId}/invite-token"), new APIRequestContextOptions
        {
            Headers = Headers(bearerToken),
        });

    public Task<IAPIResponse> GetInviteAsync(string token) =>
        GetAsync(Url($"/guest/invites/{token}"));

    public Task<IAPIResponse> GetSessionsAsync(string groupReservationId, string? bearerToken) =>
        GetAsync(Url($"/guest/groups/{groupReservationId}/sessions"), new APIRequestContextOptions
        {
            Headers = Headers(bearerToken),
        });

    public Task<IAPIResponse> GetStatusAsync(string groupReservationId) =>
        GetAsync(Url($"/guest/groups/{groupReservationId}/status"));

    public Task<IAPIResponse> GetOwnerSessionIdAsync(string reservationId, string? bearerToken) =>
        GetAsync(Url($"/reservations/{reservationId}/guest-session-id"), new APIRequestContextOptions
        {
            Headers = Headers(bearerToken),
        });

    public Task<IAPIResponse> SubmitAsync(string groupReservationId, string reservationId, string? bearerToken) =>
        Api.PostAsync(Url($"/group-reservations/{groupReservationId}/submit"), new APIRequestContextOptions
        {
            DataString = Serialize(new SubmitGroupPreOrderRequest(reservationId)),
            Headers = Headers(bearerToken),
        });

    public Task<IAPIResponse> ReplaceGuestDishesAsync(
        string groupReservationId, string guestSessionId, IReadOnlyList<GuestDishRequest> items, string? bearerToken) =>
        PutAsync(Url($"/guest/groups/{groupReservationId}/sessions/{guestSessionId}/dishes"), new APIRequestContextOptions
        {
            DataString = Serialize(new ReplaceGuestDishesRequest(items)),
            Headers = Headers(bearerToken),
        });

    public Task<IAPIResponse> RemindGuestAsync(string groupReservationId, string guestSessionId, string? bearerToken) =>
        Api.PostAsync(Url($"/guest/groups/{groupReservationId}/sessions/{guestSessionId}/reminders"), new APIRequestContextOptions
        {
            Headers = Headers(bearerToken),
        });

    public Task<IAPIResponse> RemindPendingAsync(string groupReservationId, string? bearerToken) =>
        Api.PostAsync(Url($"/guest/groups/{groupReservationId}/reminders"), new APIRequestContextOptions
        {
            Headers = Headers(bearerToken),
        });

    public Task<IAPIResponse> GetMenuAsync(string groupReservationId, string? bearerToken) =>
        GetAsync(Url($"/guest/groups/{groupReservationId}/menu"), new APIRequestContextOptions
        {
            Headers = Headers(bearerToken),
        });

    public Task<IAPIResponse> GetActivityAsync(string groupReservationId, string? bearerToken) =>
        GetAsync(Url($"/guest/groups/{groupReservationId}/activity"), new APIRequestContextOptions
        {
            Headers = Headers(bearerToken),
        });

    public Task<IAPIResponse> RemoveParticipantAsync(string groupReservationId, string guestSessionId, string? bearerToken) =>
        Api.DeleteAsync(Url($"/guest/groups/{groupReservationId}/sessions/{guestSessionId}"), new APIRequestContextOptions
        {
            Headers = Headers(bearerToken),
        });
}
