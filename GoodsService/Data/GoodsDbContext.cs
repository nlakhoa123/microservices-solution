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
            new Product { Id = 1, Name = "1 Month", Description = "Unlock more limit for your link", Price = 15, Stock = 1000, Category = "Package", ImageUrl = "https://via.placeholder.com/200" },
            new Product { Id = 2, Name = "2 Months", Description = "If 1 month not enough, you can try 2 months", Price = 29, Stock = 200, Category = "Package", ImageUrl = "https://via.placeholder.com/200" },
            new Product { Id = 3, Name = "3 Months", Description = "Oh, 2 months isn't enough?", Price = 100, Stock = 100, Category = "Package", ImageUrl = "https://via.placeholder.com/200" },
            new Product { Id = 4, Name = "1 Year", Description = "Here's a year of unlimited access", Price = 150, Stock = 30, Category = "Package", ImageUrl = "https://via.placeholder.com/200" }
        );
    }
}
