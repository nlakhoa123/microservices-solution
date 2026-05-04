using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using UrlShortenerService.Data;
using UrlShortenerService.DTOs;
using UrlShortenerService.Models;

namespace UrlShortenerService.Services;

public interface IUrlShortenerService
{
    Task<ShortUrlResponseDto> CreateAsync(CreateShortUrlDto dto, int? userId);
    Task<string?> ResolveAsync(string code);
    Task<List<ShortUrlResponseDto>> GetUserUrlsAsync(int userId);
    Task<bool> DeleteAsync(string code, int userId);
}

public class UrlShortenerSvc : IUrlShortenerService
{
    private readonly UrlDbContext _db;
    private readonly IDatabase? _redis;
    private readonly IConfiguration _config;
    private const int CacheSeconds = 3600;

    public UrlShortenerSvc(UrlDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;

        try
        {
            var redisConn = ConnectionMultiplexer.Connect(config.GetConnectionString("Redis") ?? "redis:6379");
            _redis = redisConn.GetDatabase();
        }
        catch
        {
            _redis = null; // Gracefully degrade if Redis is unavailable
        }
    }

    public async Task<ShortUrlResponseDto> CreateAsync(CreateShortUrlDto dto, int? userId)
    {
        if (!Uri.TryCreate(dto.OriginalUrl, UriKind.Absolute, out _))
            throw new ArgumentException("Invalid URL format.");

        var code = GenerateCode();
        var shortUrl = new ShortUrl
        {
            OriginalUrl = dto.OriginalUrl,
            ShortCode = code,
            UserId = userId,
            ExpiresAt = dto.ExpiresInDays.HasValue
                ? DateTime.UtcNow.AddDays(dto.ExpiresInDays.Value)
                : null
        };

        _db.ShortUrls.Add(shortUrl);
        await _db.SaveChangesAsync();

        // Cache in Redis
        if (_redis != null)
            await _redis.StringSetAsync($"url:{code}", dto.OriginalUrl, TimeSpan.FromSeconds(CacheSeconds));

        var baseUrl = _config["BaseUrl"] ?? "http://localhost:5002";
        return MapToDto(shortUrl, baseUrl);
    }

    public async Task<string?> ResolveAsync(string code)
    {
        // Check Redis cache first
        if (_redis != null)
        {
            var cached = await _redis.StringGetAsync($"url:{code}");
            if (!cached.IsNullOrEmpty)
            {
                // Increment click count in background
                _ = Task.Run(async () =>
                {
                    var entity = await _db.ShortUrls.FirstOrDefaultAsync(u => u.ShortCode == code);
                    if (entity != null) { entity.ClickCount++; await _db.SaveChangesAsync(); }
                });
                return cached.ToString();
            }
        }

        var shortUrl = await _db.ShortUrls
            .FirstOrDefaultAsync(u => u.ShortCode == code && u.IsActive);

        if (shortUrl == null) return null;
        if (shortUrl.ExpiresAt.HasValue && shortUrl.ExpiresAt < DateTime.UtcNow) return null;

        shortUrl.ClickCount++;
        await _db.SaveChangesAsync();

        // Repopulate cache
        if (_redis != null)
            await _redis.StringSetAsync($"url:{code}", shortUrl.OriginalUrl, TimeSpan.FromSeconds(CacheSeconds));

        return shortUrl.OriginalUrl;
    }

    public async Task<List<ShortUrlResponseDto>> GetUserUrlsAsync(int userId)
    {
        var baseUrl = _config["BaseUrl"] ?? "http://localhost:5002";
        var urls = await _db.ShortUrls
            .Where(u => u.UserId == userId && u.IsActive)
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();
        return urls.Select(u => MapToDto(u, baseUrl)).ToList();
    }

    public async Task<bool> DeleteAsync(string code, int userId)
    {
        var url = await _db.ShortUrls.FirstOrDefaultAsync(u => u.ShortCode == code && u.UserId == userId);
        if (url == null) return false;
        url.IsActive = false;
        await _db.SaveChangesAsync();

        if (_redis != null)
            await _redis.KeyDeleteAsync($"url:{code}");

        return true;
    }

    private static string GenerateCode()
    {
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 7)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    private static ShortUrlResponseDto MapToDto(ShortUrl u, string baseUrl) =>
        new(u.Id, u.OriginalUrl, u.ShortCode, $"{baseUrl}/r/{u.ShortCode}",
            u.ClickCount, u.CreatedAt, u.ExpiresAt);
}
