namespace Restaurant_Aqa_Framework.Screenplay.GroupOrders;

public sealed class GuestCart(string guestName, string booking, string sessionId, string sessionToken)
{
    private readonly Dictionary<string, int> _items = new(StringComparer.Ordinal);

    public string GuestName { get; } = guestName;
    public string Booking { get; } = booking;
    public string SessionId { get; } = sessionId;
    public string SessionToken { get; } = sessionToken;
    public bool IsSubmitted { get; private set; }
    public bool IsRemoved { get; private set; }

    public IReadOnlyDictionary<string, int> Items => _items;

    public void Add(string label, int quantity) =>
        _items[label] = (_items.TryGetValue(label, out var current) ? current : 0) + quantity;

    public void Remove(string label, int quantity)
    {
        var left = (_items.TryGetValue(label, out var current) ? current : 0) - quantity;
        if (left > 0)
            _items[label] = left;
        else
            _items.Remove(label);
    }

    public void MarkSubmitted() => IsSubmitted = true;

    public void MarkRemoved() => IsRemoved = true;
}
