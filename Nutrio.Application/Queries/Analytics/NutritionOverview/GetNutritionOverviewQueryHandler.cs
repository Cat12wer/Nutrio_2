// Nutrio.Application/Queries/Analytics/NutritionOverview/GetNutritionOverviewQueryHandler.cs
using MediatR;
using Nutrio.Domain.Interfaces;

namespace Nutrio.Application.Queries.Analytics.NutritionOverview;

public class GetNutritionOverviewQueryHandler : IRequestHandler<GetNutritionOverviewQuery, NutritionOverviewDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IFoodEntryRepository _foodEntryRepository;
    private readonly IBodyMetricRepository _bodyMetricRepository;

    public GetNutritionOverviewQueryHandler(
        IUserRepository userRepository,
        IFoodEntryRepository foodEntryRepository,
        IBodyMetricRepository bodyMetricRepository)
    {
        _userRepository = userRepository;
        _foodEntryRepository = foodEntryRepository;
        _bodyMetricRepository = bodyMetricRepository;
    }

    public async Task<NutritionOverviewDto> Handle(GetNutritionOverviewQuery request, CancellationToken cancellationToken)
    {
        // 1. Отримуємо дані користувача та його норму
        var user = await _userRepository.GetByIdAsync(request.UserId);
        var latestMetrics = await _bodyMetricRepository.GetLatestByUserIdAsync(request.UserId);

        if (user == null || latestMetrics == null)
            throw new UnauthorizedAccessException("Користувача або заміри не знайдено.");

        // Норма калорій (ціль)
        decimal targetCalories = user.CalculateDailyCalorieNorm(latestMetrics.Metrics.Weight, latestMetrics.Metrics.Height);

        // 2. Отримуємо всі записи про їжу за вказаний період
        var entries = await _foodEntryRepository.GetByUserIdAndDateRangeAsync(
            request.UserId, request.StartDate.Date, request.EndDate.Date);

        // Групуємо записи по днях, щоб порахувати денні суми
        var dailyCalories = entries
            .GroupBy(e => e.Date.Date)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(e => e.GetTotalNutrients().Calories)
            );

        // Загальна кількість днів у вибраному періоді
        int totalDays = (request.EndDate.Date - request.StartDate.Date).Days + 1;

        // 3. Розрахунок середніх калорій та дефіциту
        decimal totalConsumed = dailyCalories.Values.Sum();

        // Ділимо на кількість днів З ЗАПИСАМИ (або на totalDays, залежно від бізнес-логіки. Краще на дні із записами)
        int daysWithLogs = dailyCalories.Count > 0 ? dailyCalories.Count : 1;
        int averageDailyCalories = (int)Math.Round(totalConsumed / daysWithLogs);

        // Дефіцит: скільки в середньому недоїли до норми (результат з мінусом, як на макеті)
        int averageDailyDeficit = averageDailyCalories - (int)targetCalories;
        // Якщо людина на наборі маси, і споживає більше, це буде профіцит (додатнє число)

        // 4. Розрахунок дотримання раціону (Compliance)
        int daysInNorm = 0;
        foreach (var dailySum in dailyCalories.Values)
        {
            // Вважаємо день "у нормі", якщо калорії не перевищують норму (або знаходяться в межах похибки +50 ккал)
            if (dailySum <= targetCalories + 50m)
            {
                daysInNorm++;
            }
        }

        int compliancePercentage = totalDays > 0
            ? (int)Math.Round((double)daysInNorm / totalDays * 100)
            : 0;

        // 5. Розрахунок зниження ваги (Weight Loss)
        // Отримуємо історію ваги, щоб порівняти перше і останнє значення в періоді
        var weightHistory = await _bodyMetricRepository.GetHistoryByUserIdAsync(request.UserId);

        var historyInPeriod = weightHistory
            .Where(w => w.DateOfEntry.Date >= request.StartDate.Date && w.DateOfEntry.Date <= request.EndDate.Date)
            .OrderBy(w => w.DateOfEntry)
            .ToList();

        decimal weightLoss = 0m;
        if (historyInPeriod.Count >= 2)
        {
            var firstWeight = historyInPeriod.First().Metrics.Weight;
            var lastWeight = historyInPeriod.Last().Metrics.Weight;
            weightLoss = Math.Round(lastWeight - firstWeight, 1); // Наприклад: 78.4 - 80.5 = -2.1 кг
        }

        // 6. Повертаємо готовий результат
        return new NutritionOverviewDto(
            AverageDailyCalories: averageDailyCalories,
            AverageDailyDeficit: averageDailyDeficit,
            WeightLossKg: weightLoss,
            CompliancePercentage: compliancePercentage,
            DaysInNorm: daysInNorm,
            TotalDaysInPeriod: totalDays
        );
    }
}