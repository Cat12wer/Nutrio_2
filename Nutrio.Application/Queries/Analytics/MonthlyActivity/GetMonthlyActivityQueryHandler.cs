using MediatR;
using Nutrio.Domain.Interfaces;

namespace Nutrio.Application.Queries.Analytics.MonthlyActivity;

public class GetMonthlyActivityQueryHandler : IRequestHandler<GetMonthlyActivityQuery, MonthlyActivityDTO>
{
    private readonly IFoodEntryRepository _foodEntryRepository;

    public GetMonthlyActivityQueryHandler(IFoodEntryRepository foodEntryRepository)
    {
        _foodEntryRepository = foodEntryRepository;
    }

    public async Task<MonthlyActivityDTO> Handle(GetMonthlyActivityQuery request, CancellationToken cancellationToken)
    {
        // 1. Визначаємо перший і останній день запитуваного місяця
        var startDate = new DateTime(request.Year, request.Month, 1);
        int daysInMonth = DateTime.DaysInMonth(request.Year, request.Month);
        var endDate = startDate.AddDays(daysInMonth - 1);

        // 2. Отримуємо всі записи про їжу за цей місяць одним запитом
        var entries = await _foodEntryRepository.GetByUserIdAndDateRangeAsync(
            request.UserId, startDate, endDate);

        // 3. Знаходимо всі унікальні дати, коли користувач вносив їжу.
        // Використовуємо HashSet для максимально швидкого пошуку далі.
        var daysWithEntries = entries
            .Select(e => e.Date.Date)
            .Distinct()
            .ToHashSet();

        var daysList = new List<MonthlyActivityDayDTO>();

        // 4. Формуємо масив для абсолютно КОЖНОГО дня в цьому місяці
        for (int i = 0; i < daysInMonth; i++)
        {
            var currentDate = startDate.AddDays(i);

            // Перевіряємо, чи є ця дата в нашому списку дат із записами
            bool hasEntries = daysWithEntries.Contains(currentDate);

            daysList.Add(new MonthlyActivityDayDTO(currentDate, hasEntries));
        }

        // 5. Повертаємо готовий масив днів на фронтенд
        return new MonthlyActivityDTO(daysList);
    }
}