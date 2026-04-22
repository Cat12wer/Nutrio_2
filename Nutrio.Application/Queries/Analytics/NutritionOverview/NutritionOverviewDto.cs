// Nutrio.Application/Queries/Analytics/NutritionOverview/NutritionOverviewDto.cs
namespace Nutrio.Application.Queries.Analytics.NutritionOverview;

public record NutritionOverviewDto(
    int AverageDailyCalories,
    int AverageDailyDeficit,
    decimal WeightLossKg,
    int CompliancePercentage,
    int DaysInNorm,
    int TotalDaysInPeriod
);