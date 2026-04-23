using Microsoft.EntityFrameworkCore;
using Nutrio.Domain.Entities;
using Nutrio.Domain.Interfaces;

namespace Nutrio.Infrastructure.Persistence.Repositories;

public class ProductRepository : Repository<Product, int>, IProductRepository
{
    public ProductRepository(NutrioDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Product>> SearchByNameAsync(string name)
    {
        var searchTerm = name.ToLower();
        return await _dbSet
            .Where(p => p.ProductName.ToLower().Contains(searchTerm))
            .ToListAsync();
    }
}
