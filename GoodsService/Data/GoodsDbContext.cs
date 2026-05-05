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
            new Product { Id = 1, Name = "Laptop Pro", Description = "High-performance laptop", Price = 1299.99m, Stock = 50, Category = "Electronics", ImageUrl = "https://via.placeholder.com/200" },
            new Product { Id = 2, Name = "Wireless Mouse", Description = "Ergonomic wireless mouse", Price = 29.99m, Stock = 200, Category = "Accessories", ImageUrl = "https://via.placeholder.com/200" },
            new Product { Id = 3, Name = "USB-C Hub", Description = "7-in-1 USB-C hub", Price = 49.99m, Stock = 100, Category = "Accessories", ImageUrl = "https://via.placeholder.com/200" },
            new Product { Id = 4, Name = "Monitor 27\"", Description = "4K IPS display", Price = 399.99m, Stock = 30, Category = "Electronics", ImageUrl = "https://via.placeholder.com/200" }
        );
    }
}
