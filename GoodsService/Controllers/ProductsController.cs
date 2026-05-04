using GoodsService.Data;
using GoodsService.DTOs;
using GoodsService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoodsService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly GoodsDbContext _db;
    public ProductsController(GoodsDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? category)
    {
        var query = _db.Products.Where(p => p.IsActive);
        if (!string.IsNullOrEmpty(category))
            query = query.Where(p => p.Category == category);

        var products = await query.Select(p => new ProductDto(
            p.Id, p.Name, p.Description, p.Price, p.Stock, p.Category, p.ImageUrl
        )).ToListAsync();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var p = await _db.Products.FindAsync(id);
        if (p == null || !p.IsActive) return NotFound();
        return Ok(new ProductDto(p.Id, p.Name, p.Description, p.Price, p.Stock, p.Category, p.ImageUrl));
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
    {
        var product = new Product
        {
            Name = dto.Name, Description = dto.Description,
            Price = dto.Price, Stock = dto.Stock,
            Category = dto.Category, ImageUrl = dto.ImageUrl
        };
        _db.Products.Add(product);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = product.Id },
            new ProductDto(product.Id, product.Name, product.Description, product.Price, product.Stock, product.Category, product.ImageUrl));
    }
}
