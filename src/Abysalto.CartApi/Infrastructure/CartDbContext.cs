using AbySalto.CartApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace AbySalto.CartApi.Infrastructure;

public class CartDbContext : DbContext
{
    public CartDbContext(DbContextOptions<CartDbContext> options) : base(options) { }

    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cart>(b =>
        {
            b.ToTable("carts");
            b.HasKey(x => x.Id);

            b.Property(x => x.UserId).IsRequired();
            b.Property(x => x.Currency).HasMaxLength(3).IsRequired();
            b.Property(x => x.Status).IsRequired();
            b.Property(x => x.CreatedAt).IsRequired();
            b.Property(x => x.UpdatedAt).IsRequired();
            b.Property(x => x.Version).IsRequired();

            b.HasMany(x => x.Items)
                .WithOne()
                .HasForeignKey(i => i.CartId);

            b.HasIndex(x => new { x.UserId, x.Status });
        });

        modelBuilder.Entity<CartItem>(b =>
        {
            b.ToTable("cart_items");
            b.HasKey(x => x.Id);

            b.Property(x => x.Sku).IsRequired();
            b.Property(x => x.Quantity).IsRequired();
            b.Property(x => x.UnitPrice).HasColumnType("numeric(12,2)").IsRequired();
            b.Property(x => x.CreatedAt).IsRequired();

            b.HasIndex(x => x.CartId);
        });
    }
}
