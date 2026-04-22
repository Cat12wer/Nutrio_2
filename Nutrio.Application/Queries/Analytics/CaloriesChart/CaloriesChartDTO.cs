namespace Nutrio.Application.Queries.Analytics.CaloriesChart;

// DTO для окремого дня (стовпчика на графіку)
public record DailyCaloriesDTO(
    DateTime Date,
    decimal ConsumedCalories
);

// Головний DTO для всього віджета
public record CaloriesChartDTO(
    decimal DailyNorm, // Поточна денна норма калорій (для малювання горизонтальної лінії)
    List<DailyCaloriesDTO> Days // Масив днів для графіка (від StartDate до EndDate)
);