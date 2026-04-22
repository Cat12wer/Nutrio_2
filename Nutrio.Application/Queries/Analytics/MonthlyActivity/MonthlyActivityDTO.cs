namespace Nutrio.Application.Queries.Analytics.MonthlyActivity;

// DTO для окремого дня в календарі
public record MonthlyActivityDayDTO(
    DateTime Date,
    bool HasEntries
);

// Головний DTO, який повертає список днів для вибраного місяця
public record MonthlyActivityDTO(
    List<MonthlyActivityDayDTO> Days
);