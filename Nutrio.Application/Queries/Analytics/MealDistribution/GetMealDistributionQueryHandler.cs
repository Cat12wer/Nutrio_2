using MediatR;
using Nutrio.Domain.Enums;
using Nutrio.Domain.Interfaces;

namespace Nutrio.Application.Queries.Analytics.MealDistribution;

public class GetMealDistributionQueryHandler : IRequestHandler<GetMealDistributionQuery, MealDistributionDTO>
{
    private readonly IFoodEntryRepository _foodEntryRepository;

    public GetMealDistributionQueryHandler(IFoodEntryRepository foodEntryRepository)
    {
        _foodEntryRepository = foodEntryRepository;
    }

    public async Task<MealDistributionDTO> Handle(GetMealDistributionQuery request, CancellationToken cancellationToken)
    {
        // 1. Отримуємо всі записи про їжу за вказаний період
        var entries = await _foodEntryRepository.GetByUserIdAndDateRangeAsync(
            request.UserId, request.StartDate.Date, request.EndDate.Date);

        // 2. Рахуємо загальну кількість калорій за період (це наші 100%)
        decimal totalCalories = entries.Sum(e => e.GetTotalNutrients().Calories);

        // 3. Визначаємо кількість днів ІЗ ЗАПИСАМИ для розрахунку середнього арифметичного
        var uniqueDaysWithLogs = entries.Select(e => e.Date.Date).Distinct().Count();
        int daysDivisor = uniqueDaysWithLogs > 0 ? uniqueDaysWithLogs : 1;

        var mealItems = new List<MealDistributionItemDTO>();

        // 4. Проходимося по всіх існуючих типах прийомів їжі (enum MealType)
        foreach (MealType mealType in Enum.GetValues(typeof(MealType)))
        {
            // Підсумовуємо калорії ТІЛЬКИ для поточного прийому їжі
            decimal mealTotalCalories = entries
                .Where(e => e.MealType == mealType)
                .Sum(e => e.GetTotalNutrients().Calories);

            // Рахуємо середнє значення калорій на день для цього прийому
            int avgCalories = (int)Math.Round(mealTotalCalories / daysDivisor);

            // Рахуємо відсоток від загального раціону
            int percentage = 0;
            if (totalCalories > 0)
            {
                percentage = (int)Math.Round((mealTotalCalories / totalCalories) * 100);
            }

            mealItems.Add(new MealDistributionItemDTO(mealType, avgCalories, percentage));
        }

        // 5. Сортуємо в логічному порядку (Сніданок -> Перекус 1 -> Обід -> Перекус 2 -> Вечеря)
        mealItems = mealItems.OrderBy(m => (int)m.MealType).ToList();

        return new MealDistributionDTO(mealItems);
    }
}