using Nutrio.Domain.Enums;

namespace Nutrio.Application.Queries.Analytics.MealDistribution;

// DTO для окремого прийому їжі (один рядок на графіку)
public record MealDistributionItemDTO(
    MealType MealType,
    int AverageCalories,
    int Percentage
);

// Головний DTO для віджета
public record MealDistributionDTO(
    List<MealDistributionItemDTO> Meals
);