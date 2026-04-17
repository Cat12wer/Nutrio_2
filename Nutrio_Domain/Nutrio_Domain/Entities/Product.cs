using Nutrio.Domain.Common;
using Nutrio.Domain.ValueObjects;

namespace Nutrio.Domain.Entities;

public class Product : Entity<int>
{
    public string ProductName { get; private set; }
    public Nutrients NutrientsPer100g { get; private set; }

    public Product(int id, string productName, Nutrients nutrients)
    {
        Id = id;
        ProductName = productName;
        NutrientsPer100g = nutrients;
    }

    private Product() { }
}