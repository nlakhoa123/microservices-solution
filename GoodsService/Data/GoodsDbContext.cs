using GoodsService.Models;
using Microsoft.EntityFrameworkCore;

namespace GoodsService.Data;

public class GoodsDbContext : DbContext
{
    public GoodsDbContext(DbContextOptions<GoodsDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Discount> Discounts => Set<Discount>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>()
            .HasMany(o => o.Items)
            .WithOne(i => i.Order)
            .HasForeignKey(i => i.OrderId);

        modelBuilder.Entity<Discount>()
            .HasIndex(d => d.Code).IsUnique();

        // Seed data
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "1 month", Description = "More limit", Price = 15m, Stock = 50, Category = "Pack", ImageUrl = "https://via.placeholder.com/200" },
            new Product { Id = 2, Name = "2 months", Description = "Next stage", Price = 29m, Stock = 200, Category = "Pack", ImageUrl = "https://via.placeholder.com/200" },
            new Product { Id = 3, Name = "3 months", Description = "Nearby", Price = 40m, Stock = 100, Category = "Pack", ImageUrl = "https://via.placeholder.com/200" },
            new Product { Id = 4, Name = "1 year\"", Description = "Power of year", Price = 150m, Stock = 30, Category = "Pack", ImageUrl = "https://via.placeholder.com/200" }
        );
    }
}
