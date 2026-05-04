namespace GoodsService.DTOs;

public record ProductDto(int Id, string Name, string Description, decimal Price, int Stock, string Category, string ImageUrl);
public record CreateProductDto(string Name, string Description, decimal Price, int Stock, string Category, string ImageUrl);

public record DiscountDto(int Id, string Code, decimal Percentage, int CustomerId, bool IsUsed, DateTime ExpiresAt);
public record CreateDiscountDto(string Code, decimal Percentage, int CustomerId, int ExpiresInDays);
public record ApplyDiscountDto(string Code, int CustomerId);
public record DiscountResultDto(bool Valid, decimal Percentage, string Message);

public record OrderItemDto(int ProductId, int Quantity);
public record CreateOrderDto(int CustomerId, List<OrderItemDto> Items, string? DiscountCode, string PaymentMethod);
public record OrderResponseDto(
    int Id, int CustomerId, decimal TotalAmount, decimal DiscountAmount, 
    decimal FinalAmount, string Status, string PaymentMethod, DateTime CreatedAt,
    List<OrderItemDetailDto> Items
);
public record OrderItemDetailDto(string ProductName, decimal UnitPrice, int Quantity, decimal Subtotal);
