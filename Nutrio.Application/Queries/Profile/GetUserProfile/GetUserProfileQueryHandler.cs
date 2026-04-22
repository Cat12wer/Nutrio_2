using MediatR;
using Nutrio.Domain.Interfaces;

namespace Nutrio.Application.Queries.Profile.GetUserProfile;

public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, UserProfileDTO>
{
    private readonly IUserRepository _userRepository;
    private readonly IBodyMetricRepository _bodyMetricRepository;
    private readonly IFoodEntryRepository _foodEntryRepository;

    public GetUserProfileQueryHandler(
        IUserRepository userRepository,
        IBodyMetricRepository bodyMetricRepository,
        IFoodEntryRepository foodEntryRepository)
    {
        _userRepository = userRepository;
        _bodyMetricRepository = bodyMetricRepository;
        _foodEntryRepository = foodEntryRepository;
    }

    public async Task<UserProfileDTO> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        // 1. Отримуємо користувача
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user == null)
            throw new UnauthorizedAccessException("Користувача не знайдено.");

        // 2. Отримуємо метрики тіла
        var metricsHistory = await _bodyMetricRepository.GetHistoryByUserIdAsync(request.UserId);
        var latestMetrics = metricsHistory.OrderByDescending(m => m.DateOfEntry).FirstOrDefault();

        if (latestMetrics == null)
            throw new InvalidOperationException("Метрики тіла не знайдені. Пройдіть онбординг.");

        // Розрахунок скинутої ваги
        var firstMetrics = metricsHistory.OrderBy(m => m.DateOfEntry).First();
        decimal weightLost = Math.Round(firstMetrics.Metrics.Weight - latestMetrics.Metrics.Weight, 1);

        // 3. Отримуємо всі записи їжі для статистики
        var allEntries = await _foodEntryRepository.GetAllAsync(); // В ідеалі додати метод GetCountByUserId
        var userEntries = allEntries.Where(e => e.UserId == request.UserId).ToList();

        int totalEntriesCount = userEntries.Count;

        // Розрахунок серії (Streak)
        int streak = CalculateStreak(userEntries.Select(e => e.Date.Date).Distinct().ToList());

        // 4. Розраховуємо цілі (Норми)
        decimal targetCalories = user.CalculateDailyCalorieNorm(latestMetrics.Metrics.Weight, latestMetrics.Metrics.Height);
        decimal targetProteins = Math.Round((targetCalories * 0.3m) / 4m);
        decimal targetFats = Math.Round((targetCalories * 0.3m) / 9m);
        decimal targetCarbs = Math.Round((targetCalories * 0.4m) / 4m);

        // 5. Формуємо DTO
        string initials = $"{user.Name.FirstOrDefault()}{user.LastName.FirstOrDefault()}".ToUpper();
        string fullName = $"{user.Name} {user.LastName}";

        return new UserProfileDTO(
            Id: user.Id,
            Initials: initials,
            FullName: fullName,
            Email: user.Email.Value,
            Stats: new UserStatsDTO(streak, weightLost, totalEntriesCount),
            Goals: new UserGoalsDTO(targetCalories, targetProteins, targetFats, targetCarbs, 2.5m),
            Body: new UserBodyDTO(user.GetAge(), latestMetrics.Metrics.Weight, user.TargetWeight, user.WeightGoal)
        );
    }

    // Допоміжний метод для розрахунку серії
    private int CalculateStreak(List<DateTime> uniqueDatesWithLogs)
    {
        if (!uniqueDatesWithLogs.Any()) return 0;

        var dates = uniqueDatesWithLogs.OrderByDescending(d => d).ToList();
        var today = DateTime.UtcNow.Date;

        int streak = 0;
        DateTime expectedDate = dates.First() == today ? today : today.AddDays(-1);

        foreach (var date in dates)
        {
            if (date == expectedDate)
            {
                streak++;
                expectedDate = expectedDate.AddDays(-1);
            }
            else if (date < expectedDate)
            {
                break; // Серія перервалась
            }
        }

        return streak;
    }
}