using Nutrio.Domain.Entities;

namespace Nutrio.Domain.Interfaces;

public interface IProductRepository : IRepository<Product, int>
{
    Task<IReadOnlyList<Product>> SearchByNameAsync(string name);
}