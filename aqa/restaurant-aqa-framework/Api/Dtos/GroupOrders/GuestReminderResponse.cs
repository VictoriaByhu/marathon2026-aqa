namespace Restaurant_Aqa_Framework.Api.Dtos.GroupOrders;

public sealed record GuestReminderResponse(string GuestSessionId, string GuestName, DateTime RemindedAt);
