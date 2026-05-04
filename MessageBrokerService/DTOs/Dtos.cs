namespace MessageBrokerService.DTOs;

public record SendContactDto(string Name, string Email, string Subject, string Body, int? CustomerId);
public record ContactMessageDto(int Id, string Name, string Email, string Subject, string Body, int? CustomerId, string Status, DateTime CreatedAt);
