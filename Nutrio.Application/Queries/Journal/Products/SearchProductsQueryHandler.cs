using MediatR;
using Nutrio.Domain.Interfaces;

namespace Nutrio.Application.Queries.Journal.Products;

public class SearchProductsQueryHandler : IRequestHandler<SearchProductsQuery, List<ProductDto>>
{
    private readonly IProductRepository _productRepository;

    public SearchProductsQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<List<ProductDto>> Handle(SearchProductsQuery request, CancellationToken cancellationToken)
    {
        // Якщо рядок пошуку порожній, можна повертати порожній список або найпопулярніші продукти
        if (string.IsNullOrWhiteSpace(request.SearchTerm))
            return new List<ProductDto>();

        var products = await _productRepository.SearchByNameAsync(request.SearchTerm);

        // Мапимо доменні сутності у DTO для фронтенду
        return products.Select(p => new ProductDto(
            p.Id,
            p.ProductName,
            Math.Round(p.NutrientsPer100g.Calories)
        )).ToList();
    }
}