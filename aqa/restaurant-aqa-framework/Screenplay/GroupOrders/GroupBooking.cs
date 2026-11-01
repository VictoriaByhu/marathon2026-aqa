namespace Restaurant_Aqa_Framework.Screenplay.GroupOrders;

public sealed record GroupBooking(string Name, string ReservationId, string GroupReservationId, int Guests, DateTime StartTime);
