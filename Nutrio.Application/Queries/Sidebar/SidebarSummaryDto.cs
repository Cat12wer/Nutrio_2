using Nutrio.Domain.Enums;

namespace Nutrio.Application.Queries.Users.GetSidebarSummary;

public record SidebarSummaryDto(
    string Name,
    string LastName,
    WeightGoal Goal,
    decimal DailyCalorieNorm,
    decimal ConsumedCalories
);