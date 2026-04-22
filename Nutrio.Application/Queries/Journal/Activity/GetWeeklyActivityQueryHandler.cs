// Nutrio.Application/Queries/Journal/Activity/GetWeeklyActivityQueryHandler.cs
using MediatR;
using Nutrio.Domain.Interfaces;

namespace Nutrio.Application.Queries.Journal.Activity;

public class GetWeeklyActivityQueryHandler : IRequestHandler<GetWeeklyActivityQuery, WeeklyActivityDto>
{
    private readonly IFoodEntryRepository _foodEntryRepository;

    public GetWeeklyActivityQueryHandler(IFoodEntryRepository foodEntryRepository)
    {
        _foodEntryRepository = foodEntryRepository;
    }

    public async Task<WeeklyActivityDto> Handle(GetWeeklyActivityQuery request, CancellationToken cancellationToken)
    {
        var targetDate = request.CurrentDate.Date;

        // 1. Визначаємо межі тижня (Понеділок - Неділя)
        int diff = (7 + (targetDate.DayOfWeek - DayOfWeek.Monday)) % 7;
        DateTime startOfWeek = targetDate.AddDays(-1 * diff).Date;
        DateTime endOfWeek = startOfWeek.AddDays(6).Date;

        // 2. Отримуємо всі записи за цей тиждень ОДНИМ запитом
        var weeklyEntries = await _foodEntryRepository.GetByUserIdAndDateRangeAsync(
            request.UserId, startOfWeek, endOfWeek);

        var weekDays = new List<DailyActivityStatusDto>();

        // 3. Формуємо масив із 7 днів для фронтенду
        for (int i = 0; i < 7; i++)
        {
            var day = startOfWeek.AddDays(i);
            // Перевіряємо, чи є хоча б один запис у цей день
            bool hasEntries = weeklyEntries.Any(e => e.Date.Date == day);
            weekDays.Add(new DailyActivityStatusDto(day, hasEntries));
        }

        // 4. Рахуємо "Серію" (Current Streak) 🔥
        // Справжній алгоритм стріку зазвичай йде від сьогоднішнього дня назад у минуле 
        // до першого порожнього дня. Для простоти ми винесемо це в окремий метод.
        int currentStreak = await CalculateStreakAsync(request.UserId, DateTime.UtcNow.Date);

        return new WeeklyActivityDto(currentStreak, weekDays);
    }

    // Допоміжний метод для розрахунку серії (скільки днів поспіль користувач вносив їжу)
    private async Task<int> CalculateStreakAsync(Guid userId, DateTime today)
    {
        int streak = 0;
        DateTime checkDate = today;

        // Перевіряємо, чи користувач вже вносив щось сьогодні
        var todayEntries = await _foodEntryRepository.GetByUserIdAndDateAsync(userId, today);
        if (!todayEntries.Any())
        {
            // Якщо сьогодні ще порожньо, стрік не обнуляється, ми просто починаємо перевірку з учорашнього дня
            checkDate = today.AddDays(-1);
        }

        // Йдемо в минуле день за днем
        while (true)
        {
            var entries = await _foodEntryRepository.GetByUserIdAndDateAsync(userId, checkDate);
            if (entries.Any())
            {
                streak++;
                checkDate = checkDate.AddDays(-1); // Крок назад у минуле
            }
            else
            {
                // Як тільки знайшли день без записів — серія перервалась
                break;
            }
        }

        return streak;
    }
}