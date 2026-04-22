using MediatR;
using Nutrio.Domain.Interfaces;

namespace Nutrio.Application.Queries.Analytics.MacrosBalance;

public class GetMacrosBalanceQueryHandler : IRequestHandler<GetMacrosBalanceQuery, MacrosBalanceDTO>
{
    private readonly IUserRepository _userRepository;
    private readonly IBodyMetricRepository _bodyMetricRepository;
    private readonly IFoodEntryRepository _foodEntryRepository;

    public GetMacrosBalanceQueryHandler(
        IUserRepository userRepository,
        IBodyMetricRepository bodyMetricRepository,
        IFoodEntryRepository foodEntryRepository)
    {
        _userRepository = userRepository;
        _bodyMetricRepository = bodyMetricRepository;
        _foodEntryRepository = foodEntryRepository;
    }

    public async Task<MacrosBalanceDTO> Handle(GetMacrosBalanceQuery request, CancellationToken cancellationToken)
    {
        // 1. Отримуємо дані користувача для розрахунку актуальної норми
        var user = await _userRepository.GetByIdAsync(request.UserId);
        var latestMetrics = await _bodyMetricRepository.GetLatestByUserIdAsync(request.UserId);

        if (user == null || latestMetrics == null)
            throw new UnauthorizedAccessException("Користувача або заміри не знайдено.");

        decimal targetCalories = user.CalculateDailyCalorieNorm(latestMetrics.Metrics.Weight, latestMetrics.Metrics.Height);

        // 2. Розраховуємо цілі (Норми) в грамах (30% білки, 30% жири, 40% вуглеводи)
        decimal targetProteins = Math.Round((targetCalories * 0.3m) / 4m);
        decimal targetFats = Math.Round((targetCalories * 0.3m) / 9m);
        decimal targetCarbs = Math.Round((targetCalories * 0.4m) / 4m);
        decimal targetFiber = 30m; // Стандартна норма клітковини

        // 3. Отримуємо всі записи про їжу за період
        var entries = await _foodEntryRepository.GetByUserIdAndDateRangeAsync(
            request.UserId, request.StartDate.Date, request.EndDate.Date);

        // 4. Підсумовуємо всі макронутрієнти за весь період
        decimal totalCalories = 0, totalProteins = 0, totalFats = 0, totalCarbs = 0, totalFiber = 0;

        // Визначаємо кількість днів ІЗ ЗАПИСАМИ для точного середнього арифметичного
        var uniqueDaysWithLogs = entries.Select(e => e.Date.Date).Distinct().Count();
        int daysDivisor = uniqueDaysWithLogs > 0 ? uniqueDaysWithLogs : 1;

        foreach (var entry in entries)
        {
            var nutrients = entry.GetTotalNutrients();
            totalCalories += nutrients.Calories;
            totalProteins += nutrients.Protein;
            totalFats += nutrients.Fat;
            totalCarbs += nutrients.Carbs;
            totalFiber += nutrients.Fiber;
        }

        // 5. Розраховуємо СЕРЕДНЄ споживання за день у грамах
        decimal avgCalories = totalCalories / daysDivisor;
        decimal avgProteins = totalProteins / daysDivisor;
        decimal avgFats = totalFats / daysDivisor;
        decimal avgCarbs = totalCarbs / daysDivisor;
        decimal avgFiber = totalFiber / daysDivisor;

        // 6. Розраховуємо ВІДСОТКИ для кругової діаграми (відштовхуючись від енергії в ккал)
        // 1г білка/вуглевода = 4 ккал, 1г жиру = 9 ккал
        int proteinPercent = 0, fatPercent = 0, carbPercent = 0;

        if (totalCalories > 0)
        {
            proteinPercent = (int)Math.Round((totalProteins * 4m) / totalCalories * 100);
            fatPercent = (int)Math.Round((totalFats * 9m) / totalCalories * 100);
            carbPercent = (int)Math.Round((totalCarbs * 4m) / totalCalories * 100);
        }

        // 7. Формуємо DTO
        return new MacrosBalanceDTO(
            AverageDailyCalories: (int)Math.Round(avgCalories),
            Carbs: new MacroStatDTO(Math.Round(avgCarbs), targetCarbs, carbPercent),
            Proteins: new MacroStatDTO(Math.Round(avgProteins), targetProteins, proteinPercent),
            Fats: new MacroStatDTO(Math.Round(avgFats), targetFats, fatPercent),
            Fiber: new MacroStatDTO(Math.Round(avgFiber), targetFiber, 0) // Клітковина зазвичай не враховується в % енергії
        );
    }
}