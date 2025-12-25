namespace AbySalto.CartApi.Domain;

public enum CartStatus
{
    Active = 1,
    Submitted = 2
}

public class Cart
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = default!;
    public CartStatus Status { get; set; } = CartStatus.Active;
    public string Currency { get; set; } = "EUR";
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    // - Optimistic concurrency marker (jednostavan demo)
    public int Version { get; set; }

    public List<CartItem> Items { get; set; } = new();
}

public class CartItem
{
    public Guid Id { get; set; }
    public Guid CartId { get; set; }
    public string Sku { get; set; } = default!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
