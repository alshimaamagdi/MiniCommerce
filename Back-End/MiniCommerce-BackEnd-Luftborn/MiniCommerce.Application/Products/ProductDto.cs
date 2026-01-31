namespace MiniCommerce.Application.Products;

public record ProductDto(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    int StockQuantity,
    Guid CategoryId,
    string? CategoryName,
    DateTime CreatedAt);

public record CreateProductDto(string Name, string? Description, decimal Price, int StockQuantity, Guid CategoryId);

public record UpdateProductDto(string Name, string? Description, decimal Price, int StockQuantity, Guid CategoryId);
