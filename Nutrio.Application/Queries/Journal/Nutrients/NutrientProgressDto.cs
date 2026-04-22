// Nutrio.Application/Queries/Journal/Nutrients/DailyNutrientsSummaryDto.cs

namespace Nutrio.Application.Queries.Journal.Nutrients;

// Допоміжний рекорд для кожної картки (Спожито / Норма)
public record NutrientProgressDto(
    decimal Consumed,
    decimal Target
);

public record DailyNutrientsSummaryDto(
    NutrientProgressDto Calories,
    NutrientProgressDto Proteins,
    NutrientProgressDto Fats,
    NutrientProgressDto Carbs,
    NutrientProgressDto Fiber
);