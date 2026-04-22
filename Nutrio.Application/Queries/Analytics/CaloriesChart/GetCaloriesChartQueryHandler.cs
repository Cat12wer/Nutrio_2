using MediatR;
using Nutrio.Domain.Interfaces;

namespace Nutrio.Application.Queries.Analytics.CaloriesChart;

public class GetCaloriesChartQueryHandler : IRequestHandler<GetCaloriesChartQuery, CaloriesChartDTO>
{
    private readonly IUserRepository _userRepository;
    private readonly IBodyMetricRepository _bodyMetricRepository;
    private readonly IFoodEntryRepository _foodEntryRepository;

    public GetCaloriesChartQueryHandler(
        IUserRepository userRepository,
        IBodyMetricRepository bodyMetricRepository,
        IFoodEntryRepository foodEntryRepository)
    {
        _userRepository = userRepository;
        _bodyMetricRepository = bodyMetricRepository;
        _foodEntryRepository = foodEntryRepository;
    }

    public async Task<CaloriesChartDTO> Handle(GetCaloriesChartQuery request, CancellationToken cancellationToken)
    {
        // 1. Отримуємо дані користувача для простої логіки лінії норми (актуальне значення)
        var user = await _userRepository.GetByIdAsync(request.UserId);
        var latestMetrics = await _bodyMetricRepository.GetLatestByUserIdAsync(request.UserId);

        if (user == null || latestMetrics == null)
            throw new UnauthorizedAccessException("Користувача або заміри не знайдено.");

        // Рахуємо актуальну денну норму "на зараз"
        decimal dailyNorm = user.CalculateDailyCalorieNorm(latestMetrics.Metrics.Weight, latestMetrics.Metrics.Height);

        // 2. Отримуємо всі записи про їжу за весь вибраний період одним запитом
        var entries = await _foodEntryRepository.GetByUserIdAndDateRangeAsync(
            request.UserId, request.StartDate.Date, request.EndDate.Date);

        // 3. Групуємо спожиті калорії по днях
        var consumedByDate = entries
            .GroupBy(e => e.Date.Date)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(e => e.GetTotalNutrients().Calories)
            );

        // 4. Генеруємо масив для Фронтенду (заповнюємо ВСІ дні в проміжку)
        var daysList = new List<DailyCaloriesDTO>();

        // Визначаємо загальну кількість днів (напр. 7 для тижня, 30 для місяця, 90 для 3 місяців)
        int totalDays = (request.EndDate.Date - request.StartDate.Date).Days;

        for (int i = 0; i <= totalDays; i++)
        {
            var currentDate = request.StartDate.Date.AddDays(i);

            // Якщо в цей день були записи - беремо їх суму, якщо ні - ставимо 0
            decimal consumed = consumedByDate.TryGetValue(currentDate, out var calories)
                ? Math.Round(calories)
                : 0m;

            daysList.Add(new DailyCaloriesDTO(currentDate, consumed));
        }

        // 5. Повертаємо готовий об'єкт для графіка
        return new CaloriesChartDTO(dailyNorm, daysList);
    }
}