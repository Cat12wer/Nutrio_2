// Nutrio.Application/Queries/Journal/Nutrients/GetDailyNutrientsQueryHandler.cs
using MediatR;
using Nutrio.Domain.Interfaces;

namespace Nutrio.Application.Queries.Journal.Nutrients;

public class GetDailyNutrientsQueryHandler : IRequestHandler<GetDailyNutrientsQuery, DailyNutrientsSummaryDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IFoodEntryRepository _foodEntryRepository;
    private readonly IBodyMetricRepository _bodyMetricRepository;

    public GetDailyNutrientsQueryHandler(
        IUserRepository userRepository,
        IFoodEntryRepository foodEntryRepository,
        IBodyMetricRepository bodyMetricRepository)
    {
        _userRepository = userRepository;
        _foodEntryRepository = foodEntryRepository;
        _bodyMetricRepository = bodyMetricRepository;
    }

    public async Task<DailyNutrientsSummaryDto> Handle(GetDailyNutrientsQuery request, CancellationToken cancellationToken)
    {
        // 1. Отримуємо дані користувача для цілей
        var user = await _userRepository.GetByIdAsync(request.UserId);
        var latestMetrics = await _bodyMetricRepository.GetLatestByUserIdAsync(request.UserId);

        if (user == null || latestMetrics == null)
            throw new UnauthorizedAccessException("Дані користувача або заміри не знайдені.");

        // 2. Розраховуємо цілі (Норми)
        decimal targetCalories = user.CalculateDailyCalorieNorm(latestMetrics.Metrics.Weight, latestMetrics.Metrics.Height);

        // Базовий розрахунок БЖВ (можна потім винести в домен). 
        // 1г білка = 4 ккал, 1г жиру = 9 ккал, 1г вуглеводів = 4 ккал
        decimal targetProteins = Math.Round((targetCalories * 0.3m) / 4m); // 30% раціону
        decimal targetFats = Math.Round((targetCalories * 0.3m) / 9m);     // 30% раціону
        decimal targetCarbs = Math.Round((targetCalories * 0.4m) / 4m);    // 40% раціону
        decimal targetFiber = 30m; // Стандартна норма клітковини 30г на день

        // 3. Отримуємо всю їжу за вказаний день
        var foodEntries = await _foodEntryRepository.GetByUserIdAndDateAsync(request.UserId, request.Date.Date);

        // 4. Підсумовуємо те, що вже з'їдено (використовуючи доменний метод GetTotalNutrients)
        decimal consumedCalories = 0, consumedProteins = 0, consumedFats = 0, consumedCarbs = 0, consumedFiber = 0;

        foreach (var entry in foodEntries)
        {
            var nutrients = entry.GetTotalNutrients();
            consumedCalories += nutrients.Calories;
            consumedProteins += nutrients.Protein;
            consumedFats += nutrients.Fat;
            consumedCarbs += nutrients.Carbs;
            consumedFiber += nutrients.Fiber;
        }

        // 5. Повертаємо готовий результат для 5 карток
        return new DailyNutrientsSummaryDto(
            Calories: new NutrientProgressDto(Math.Round(consumedCalories), targetCalories),
            Proteins: new NutrientProgressDto(Math.Round(consumedProteins), targetProteins),
            Fats: new NutrientProgressDto(Math.Round(consumedFats), targetFats),
            Carbs: new NutrientProgressDto(Math.Round(consumedCarbs), targetCarbs),
            Fiber: new NutrientProgressDto(Math.Round(consumedFiber), targetFiber)
        );
    }
}