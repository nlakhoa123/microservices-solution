namespace UserService.DTOs;

public record RegisterDto(string Username, string Email, string Password);
public record LoginDto(string Email, string Password);
public record AuthResponseDto(string Token, string Username, string Email, string Role, int UserId);
public record UserProfileDto(int Id, string Username, string Email, string Role, DateTime CreatedAt);
public record UpdateProfileDto(string? Username, string? Email);
