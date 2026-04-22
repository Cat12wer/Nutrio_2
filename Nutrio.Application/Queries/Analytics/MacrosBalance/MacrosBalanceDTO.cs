namespace Nutrio.Application.Queries.Analytics.MacrosBalance;

// DTO для окремого макронутрієнта
public record MacroStatDTO(
    decimal AverageConsumed, // Середнє споживання за день (у грамах)
    decimal Target,          // Денна ціль (у грамах)
    int Percentage           // Відсоток від загального спожитих калорій (для кругової діаграми)
);

// Головний DTO для віджета
public record MacrosBalanceDTO(
    int AverageDailyCalories, // Для центру кругової діаграми
    MacroStatDTO Carbs,
    MacroStatDTO Proteins,
    MacroStatDTO Fats,
    MacroStatDTO Fiber
);