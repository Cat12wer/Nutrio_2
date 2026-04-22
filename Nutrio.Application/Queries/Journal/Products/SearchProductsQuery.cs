using MediatR;

namespace Nutrio.Application.Queries.Journal.Products;

public record SearchProductsQuery(
    string SearchTerm
) : IRequest<List<ProductDto>>;