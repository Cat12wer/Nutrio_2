using Nutrio.Domain.Entities;
using Nutrio.Domain.Enums;

namespace Nutrio.Domain.Interfaces;

public interface IFoodEntryRepository : IRepository<FoodEntry, Guid>
{
    Task<IReadOnlyList<FoodEntry>> GetByUserIdAndDateAsync(Guid userId, DateTime date);
    Task<IReadOnlyList<FoodEntry>> GetByMealTypeAsync(Guid userId, DateTime date, MealType mealType);
    Task<IReadOnlyList<FoodEntry>> GetByUserIdAndDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate);
}