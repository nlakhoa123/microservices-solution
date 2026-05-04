using System.Security.Claims;
using GoodsService.Data;
using GoodsService.DTOs;
using GoodsService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoodsService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly GoodsDbContext _db;
    public OrdersController(GoodsDbContext db) => _db = db;

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
    {
        // Validate products and calculate total
        decimal total = 0;
        var orderItems = new List<OrderItem>();

        foreach (var item in dto.Items)
        {
            var product = await _db.Products.FindAsync(item.ProductId);
            if (product == null || !product.IsActive)
                return BadRequest(new { message = $"Product {item.ProductId} not found." });
            if (product.Stock < item.Quantity)
                return BadRequest(new { message = $"Insufficient stock for {product.Name}." });

            var subtotal = product.Price * item.Quantity;
            total += subtotal;
            orderItems.Add(new OrderItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                UnitPrice = product.Price,
                Quantity = item.Quantity,
                Subtotal = subtotal
            });
        }

        // Apply discount
        decimal discountAmt = 0;
        if (!string.IsNullOrEmpty(dto.DiscountCode))
        {
            var discount = await _db.Discounts.FirstOrDefaultAsync(d =>
                d.Code == dto.DiscountCode && d.CustomerId == dto.CustomerId && !d.IsUsed && d.ExpiresAt > DateTime.UtcNow);

            if (discount != null)
            {
                discountAmt = total * (discount.Percentage / 100);
                discount.IsUsed = true;
            }
        }

        // Deduct stock
        foreach (var item in dto.Items)
        {
            var product = await _db.Products.FindAsync(item.ProductId);
            product!.Stock -= item.Quantity;
        }

        var order = new Order
        {
            CustomerId = dto.CustomerId,
            Items = orderItems,
            TotalAmount = total,
            DiscountAmount = discountAmt,
            FinalAmount = total - discountAmt,
            DiscountCode = dto.DiscountCode,
            Status = "Paid", // Simulated instant payment
            PaymentMethod = dto.PaymentMethod
        };

        _db.Orders.Add(order);
        await _db.SaveChangesAsync();

        return Ok(MapToResponse(order));
    }

    [HttpGet("customer/{customerId}")]
    [Authorize]
    public async Task<IActionResult> GetCustomerOrders(int customerId)
    {
        var orders = await _db.Orders
            .Include(o => o.Items)
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        return Ok(orders.Select(MapToResponse));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrder(int id)
    {
        var order = await _db.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id);
        if (order == null) return NotFound();
        return Ok(MapToResponse(order));
    }

    private static OrderResponseDto MapToResponse(Order o) => new(
        o.Id, o.CustomerId, o.TotalAmount, o.DiscountAmount, o.FinalAmount,
        o.Status, o.PaymentMethod, o.CreatedAt,
        o.Items.Select(i => new OrderItemDetailDto(i.ProductName, i.UnitPrice, i.Quantity, i.Subtotal)).ToList()
    );
}
