namespace UrlShortenerService.DTOs;

public record CreateShortUrlDto(string OriginalUrl, int? ExpiresInDays);
public record ShortUrlResponseDto(
    int Id,
    string OriginalUrl,
    string ShortCode,
    string ShortUrl,
    int ClickCount,
    DateTime CreatedAt,
    DateTime? ExpiresAt
);
