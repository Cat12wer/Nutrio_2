using MediatR;
using Nutrio.Domain.Interfaces;

namespace Nutrio.Application.Queries.Analytics.WeeklyGoalsProgress;

public class GetWeeklyGoalsProgressQueryHandler : IRequestHandler<GetWeeklyGoalsProgressQuery, WeeklyGoalsProgressDTO>
{
    private readonly IUserRepository _userRepository;
    private readonly IBodyMetricRepository _bodyMetricRepository;
    private readonly IFoodEntryRepository _foodEntryRepository;

    public GetWeeklyGoalsProgressQueryHandler(
        IUserRepository userRepository,
        IBodyMetricRepository bodyMetricRepository,
        IFoodEntryRepository foodEntryRepository)
    {
        _userRepository = userRepository;
        _bodyMetricRepository = bodyMetricRepository;
        _foodEntryRepository = foodEntryRepository;
    }

    public async Task<WeeklyGoalsProgressDTO> Handle(GetWeeklyGoalsProgressQuery request, CancellationToken cancellationToken)
    {
        // 1. Отримуємо дані користувача для розрахунку норм
        var user = await _userRepository.GetByIdAsync(request.UserId);
        var latestMetrics = await _bodyMetricRepository.GetLatestByUserIdAsync(request.UserId);

        if (user == null || latestMetrics == null)
            throw new UnauthorizedAccessException("Користувача або заміри не знайдено.");

        // 2. Розраховуємо цілі
        decimal targetCalories = user.CalculateDailyCalorieNorm(latestMetrics.Metrics.Weight, latestMetrics.Metrics.Height);
        decimal targetProteins = Math.Round((targetCalories * 0.3m) / 4m);
        decimal targetFats = Math.Round((targetCalories * 0.3m) / 9m);
        decimal targetCarbs = Math.Round((targetCalories * 0.4m) / 4m);
        decimal targetFiber = 30m;

        // 3. Отримуємо записи про їжу за період
        var entries = await _foodEntryRepository.GetByUserIdAndDateRangeAsync(
            request.UserId, request.StartDate.Date, request.EndDate.Date);

        // Групуємо по днях
        var entriesByDay = entries.GroupBy(e => e.Date.Date).ToList();

        // Лічильники успішних днів
        int daysCaloriesMet = 0, daysProteinsMet = 0, daysFatsMet = 0;
        int daysCarbsMet = 0, daysFiberMet = 0, daysMealsMet = 0;

        foreach (var dayGroup in entriesByDay)
        {
            decimal dayCals = dayGroup.Sum(e => e.GetTotalNutrients().Calories);
            decimal dayProts = dayGroup.Sum(e => e.GetTotalNutrients().Protein);
            decimal dayFats = dayGroup.Sum(e => e.GetTotalNutrients().Fat);
            decimal dayCarbs = dayGroup.Sum(e => e.GetTotalNutrients().Carbs);
            decimal dayFiber = dayGroup.Sum(e => e.GetTotalNutrients().Fiber);
            int uniqueMealsCount = dayGroup.Select(e => e.MealType).Distinct().Count();

            // Логіка "Успіху" (з невеликою похибкою для реалістичності)
            // Калорії: відхилення не більше 10% від норми
            if (dayCals >= targetCalories * 0.9m && dayCals <= targetCalories * 1.1m) daysCaloriesMet++;

            // Білки: з'їдено хоча б 90% від цілі
            if (dayProts >= targetProteins * 0.9m) daysProteinsMet++;

            // Жири та Вуглеводи: не перевищено більше ніж на 10%
            if (dayFats <= targetFats * 1.1m) daysFatsMet++;
            if (dayCarbs <= targetCarbs * 1.1m) daysCarbsMet++;

            // Клітковина: більше або дорівнює 25г
            if (dayFiber >= 25m) daysFiberMet++;

            // Прийоми їжі: було мінімум 3 прийоми їжі за день (напр. Сніданок, Обід, Вечеря)
            if (uniqueMealsCount >= 3) daysMealsMet++;
        }

        // Загальна кількість днів у вибраному періоді (наприклад, 7)
        int totalDays = (request.EndDate.Date - request.StartDate.Date).Days + 1;

        return new WeeklyGoalsProgressDTO(
            Calories: new GoalProgressDTO(daysCaloriesMet, totalDays),
            Proteins: new GoalProgressDTO(daysProteinsMet, totalDays),
            Fats: new GoalProgressDTO(daysFatsMet, totalDays),
            Carbs: new GoalProgressDTO(daysCarbsMet, totalDays),
            Fiber: new GoalProgressDTO(daysFiberMet, totalDays),
            Meals: new GoalProgressDTO(daysMealsMet, totalDays)
        );
    }
}