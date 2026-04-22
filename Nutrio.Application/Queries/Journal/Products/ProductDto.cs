namespace Nutrio.Application.Queries.Journal.Products;

public record ProductDto(
    int Id,
    string ProductName,
    decimal CaloriesPer100g
);