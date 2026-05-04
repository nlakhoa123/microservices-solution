using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrlShortenerService.DTOs;
using UrlShortenerService.Services;

namespace UrlShortenerService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UrlController : ControllerBase
{
    private readonly IUrlShortenerService _service;

    public UrlController(IUrlShortenerService service) => _service = service;

    [HttpPost("shorten")]
    public async Task<IActionResult> Shorten([FromBody] CreateShortUrlDto dto)
    {
        try
        {
            var userId = GetUserId();
            var result = await _service.CreateAsync(dto, userId);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("my-urls")]
    [Authorize]
    public async Task<IActionResult> GetMyUrls()
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();
        var urls = await _service.GetUserUrlsAsync(userId.Value);
        return Ok(urls);
    }

    [HttpDelete("{code}")]
    [Authorize]
    public async Task<IActionResult> Delete(string code)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();
        var deleted = await _service.DeleteAsync(code, userId.Value);
        if (!deleted) return NotFound(new { message = "URL not found or not yours." });
        return Ok(new { message = "Deleted." });
    }

    private int? GetUserId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return claim != null ? int.Parse(claim) : null;
    }
}

[ApiController]
[Route("r")]
public class RedirectController : ControllerBase
{
    private readonly IUrlShortenerService _service;

    public RedirectController(IUrlShortenerService service) => _service = service;

    [HttpGet("{code}")]
    public async Task<IActionResult> RedirectToUrl(string code)
    {
        var url = await _service.ResolveAsync(code);
        if (url == null) return NotFound(new { message = "Short URL not found or expired." });
        return base.Redirect(url);
    }
}
