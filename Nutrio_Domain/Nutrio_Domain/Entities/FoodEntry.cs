using Nutrio.Domain.Common;
using Nutrio.Domain.ValueObjects;
using Nutrio.Domain.Enums;

namespace Nutrio.Domain.Entities;

public class FoodEntry : Entity<Guid>
{
    public Guid UserId { get; private set; }
    public int ProductId { get; private set; }
    public DateTime Date { get; private set; }
    public Quantity Quantity { get; private set; } // Об'єкт-значення для ваги [cite: 112]
    public MealType MealType { get; private set; }

    // Навігаційна властивість
    public virtual Product Product { get; private set; }

    public FoodEntry(Guid userId, int productId, DateTime date, Quantity quantity, MealType mealType)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        ProductId = productId;
        Date = date;
        Quantity = quantity;
        MealType = mealType;
    }

    [cite_start]// Динамічний розрахунок нутрієнтів "на льоту" для дашборду [cite: 86, 122]
    public Nutrients GetTotalNutrients()
    {
        return Product.NutrientsPer100g.MultiplyByQuantity((int)Quantity.Value);
    }
}