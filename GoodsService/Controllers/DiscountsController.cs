using GoodsService.Data;
using GoodsService.DTOs;
using GoodsService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoodsService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DiscountsController : ControllerBase
{
    private readonly GoodsDbContext _db;
    public DiscountsController(GoodsDbContext db) => _db = db;

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateDiscountDto dto)
    {
        if (await _db.Discounts.AnyAsync(d => d.Code == dto.Code))
            return Conflict(new { message = "Discount code already exists." });

        var discount = new Discount
        {
            Code = dto.Code,
            Percentage = dto.Percentage,
            CustomerId = dto.CustomerId,
            ExpiresAt = DateTime.UtcNow.AddDays(dto.ExpiresInDays)
        };
        _db.Discounts.Add(discount);
        await _db.SaveChangesAsync();
        return Ok(new DiscountDto(discount.Id, discount.Code, discount.Percentage, discount.CustomerId, discount.IsUsed, discount.ExpiresAt));
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate([FromBody] ApplyDiscountDto dto)
    {
        var discount = await _db.Discounts.FirstOrDefaultAsync(d =>
            d.Code == dto.Code && d.CustomerId == dto.CustomerId);

        if (discount == null)
            return Ok(new DiscountResultDto(false, 0, "Invalid discount code."));
        if (discount.IsUsed)
            return Ok(new DiscountResultDto(false, 0, "Discount code already used."));
        if (discount.ExpiresAt < DateTime.UtcNow)
            return Ok(new DiscountResultDto(false, 0, "Discount code expired."));

        return Ok(new DiscountResultDto(true, discount.Percentage, $"Discount of {discount.Percentage}% applied."));
    }

    [HttpGet("customer/{customerId}")]
    [Authorize]
    public async Task<IActionResult> GetByCustomer(int customerId)
    {
        var discounts = await _db.Discounts
            .Where(d => d.CustomerId == customerId)
            .Select(d => new DiscountDto(d.Id, d.Code, d.Percentage, d.CustomerId, d.IsUsed, d.ExpiresAt))
            .ToListAsync();
        return Ok(discounts);
    }
}
