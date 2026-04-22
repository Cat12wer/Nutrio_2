// Nutrio.Application/Queries/Journal/Meals/GetDailyFoodEntriesQueryHandler.cs
using MediatR;
using Nutrio.Domain.Enums;
using Nutrio.Domain.Interfaces;

namespace Nutrio.Application.Queries.Journal.Meals;

public class GetDailyFoodEntriesQueryHandler : IRequestHandler<GetDailyFoodEntriesQuery, List<MealGroupDto>>
{
    private readonly IFoodEntryRepository _foodEntryRepository;

    public GetDailyFoodEntriesQueryHandler(IFoodEntryRepository foodEntryRepository)
    {
        _foodEntryRepository = foodEntryRepository;
    }

    public async Task<List<MealGroupDto>> Handle(GetDailyFoodEntriesQuery request, CancellationToken cancellationToken)
    {
        // 1. Отримуємо всі записи з бази за конкретну дату
        // Важливо: переконайся, що репозиторій підтягує навігаційну властивість Product (через .Include(x => x.Product) в інфраструктурному шарі)
        var entries = await _foodEntryRepository.GetByUserIdAndDateAsync(request.UserId, request.Date.Date);

        var result = new List<MealGroupDto>();

        // 2. Проходимося по всіх існуючих типах прийомів їжі (Сніданок, Обід і т.д.)
        foreach (MealType mealType in Enum.GetValues(typeof(MealType)))
        {
            // Шукаємо страви, які належать до поточного прийому їжі
            var mealEntries = entries.Where(e => e.MealType == mealType).ToList();

            // 3. Формуємо список продуктів для DTO, використовуючи доменні методи
            var itemsDto = mealEntries.Select(e => new FoodItemDto(
                EntryId: e.Id,
                ProductName: e.Product.ProductName,
                Grams: e.Quantity.Value,
                Calories: Math.Round(e.GetTotalNutrients().Calories) // Розрахунок на льоту згідно з порцією
            )).ToList();

            // 4. Рахуємо загальні калорії для цього прийому їжі
            decimal totalMealCalories = itemsDto.Sum(i => i.Calories);

            result.Add(new MealGroupDto(
                MealType: mealType,
                TotalCalories: totalMealCalories,
                Items: itemsDto
            ));
        }

        return result;
    }
}