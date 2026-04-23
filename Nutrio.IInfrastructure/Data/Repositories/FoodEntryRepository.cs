using Microsoft.EntityFrameworkCore;
using Nutrio.Domain.Entities;
using Nutrio.Domain.Enums;
using Nutrio.Domain.Interfaces;

namespace Nutrio.Infrastructure.Persistence.Repositories;

public class FoodEntryRepository : Repository<FoodEntry, Guid>, IFoodEntryRepository
{
    public FoodEntryRepository(NutrioDbContext context) : base(context) { }

    public async Task<IReadOnlyList<FoodEntry>> GetByUserIdAndDateAsync(Guid userId, DateTime date)
    {
        return await _dbSet
            .Include(f => f.Product) // Підтягуємо дані про продукт
            .Where(f => f.UserId == userId && f.Date.Date == date.Date)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<FoodEntry>> GetByMealTypeAsync(Guid userId, DateTime date, MealType mealType)
    {
        return await _dbSet
            .Include(f => f.Product)
            .Where(f => f.UserId == userId && f.Date.Date == date.Date && f.MealType == mealType)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<FoodEntry>> GetByUserIdAndDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Include(f => f.Product)
            .Where(f => f.UserId == userId && f.Date.Date >= startDate.Date && f.Date.Date <= endDate.Date)
            .ToListAsync();
    }
}