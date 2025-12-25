using AbySalto.CartApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace AbySalto.CartApi.Infrastructure;

public class CartService
{
    private readonly CartDbContext _db;

    public CartService(CartDbContext db) => _db = db;

    public async Task<Cart> GetOrCreateActiveCartAsync(string userId, CancellationToken ct)
    {
        var cart = await _db.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId && c.Status == CartStatus.Active, ct);

        if (cart != null) return cart;

        cart = new Cart
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Status = CartStatus.Active,
            Currency = "EUR",
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
            Version = 1
        };

        _db.Carts.Add(cart);
        await _db.SaveChangesAsync(ct);
        return cart;
    }

    public async Task<Cart> GetCartAsync(Guid cartId, string userId, CancellationToken ct)
    {
        var cart = await _db.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.Id == cartId && c.UserId == userId, ct);

        return cart ?? throw new KeyNotFoundException("Cart not found");
    }

    public async Task<Cart> AddItemAsync(Guid cartId, string userId, string sku, int quantity, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(sku)) throw new ArgumentException("Sku is required");
        if (quantity <= 0) throw new ArgumentException("Quantity must be > 0");

        var cart = await _db.Carts.Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.Id == cartId && c.UserId == userId, ct)
            ?? throw new KeyNotFoundException("Cart not found");

        if (cart.Status != CartStatus.Active) throw new InvalidOperationException("Cart is not active");

        var existing = cart.Items.FirstOrDefault(i => i.Sku == sku);
        if (existing != null)
        {
            existing.Quantity += quantity;
        }
        else
        {
            cart.Items.Add(new CartItem
            {
                Id = Guid.NewGuid(),
                CartId = cart.Id,
                Sku = sku.Trim(),
                Quantity = quantity,
                UnitPrice = 9.99m, // - demo cijena
                CreatedAt = DateTimeOffset.UtcNow
            });
        }

        cart.UpdatedAt = DateTimeOffset.UtcNow;
        cart.Version += 1;

        await _db.SaveChangesAsync(ct);
        return cart;
    }
}
