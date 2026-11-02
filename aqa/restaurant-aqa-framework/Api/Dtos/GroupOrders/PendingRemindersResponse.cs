namespace Restaurant_Aqa_Framework.Api.Dtos.GroupOrders;

public sealed record PendingRemindersResponse(IReadOnlyList<GuestReminderResponse> Reminded);
