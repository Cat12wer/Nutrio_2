// Nutrio.Application/Queries/Users/GetSidebarSummary/GetUserSidebarSummaryQueryHandler.cs
using MediatR;
using Nutrio.Domain.Interfaces;

namespace Nutrio.Application.Queries.Users.GetSidebarSummary;

public class GetUserSidebarSummaryQueryHandler : IRequestHandler<GetUserSidebarSummaryQuery, SidebarSummaryDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IFoodEntryRepository _foodEntryRepository;
    private readonly IBodyMetricRepository _bodyMetricRepository;

    public GetUserSidebarSummaryQueryHandler(
        IUserRepository userRepository,
        IFoodEntryRepository foodEntryRepository,
        IBodyMetricRepository bodyMetricRepository)
    {
        _userRepository = userRepository;
        _foodEntryRepository = foodEntryRepository;
        _bodyMetricRepository = bodyMetricRepository;
    }

    public async Task<SidebarSummaryDto> Handle(GetUserSidebarSummaryQuery request, CancellationToken cancellationToken)
    {
        // 1. Отримуємо користувача
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user == null)
            throw new UnauthorizedAccessException("Користувача не знайдено.");

        // 2. Отримуємо його останні заміри тіла (щоб розрахувати норму калорій)
        var latestMetrics = await _bodyMetricRepository.GetLatestByUserIdAsync(request.UserId);

        decimal dailyNorm = 0;
        if (latestMetrics != null)
        {
            dailyNorm = user.CalculateDailyCalorieNorm(
                latestMetrics.Metrics.Weight,
                latestMetrics.Metrics.Height);
        }

        // 3. Жорстко задаємо поточну дату на бекенді (краще використовувати UTC)
        var today = DateTime.UtcNow.Date;

        // 4. Отримуємо їжу тільки за сьогодні
        var foodEntries = await _foodEntryRepository.GetByUserIdAndDateAsync(request.UserId, today);

        // 5. Підраховуємо сумарні калорії
        decimal consumedCalories = foodEntries.Sum(entry => entry.GetTotalNutrients().Calories);

        // 6. Повертаємо готовий результат
        return new SidebarSummaryDto(
            user.Name,
            user.LastName,
            user.WeightGoal,
            dailyNorm,
            Math.Round(consumedCalories)
        );
    }
}