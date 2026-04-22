namespace Nutrio.Application.Queries.Analytics.WeeklyGoalsProgress;

// Допоміжний DTO для окремої цілі (наприклад, 6 виконаних днів із 7)
public record GoalProgressDTO(
    int DaysCompleted,
    int TotalDays
);

// Головний DTO для віджета
public record WeeklyGoalsProgressDTO(
    GoalProgressDTO Calories,
    GoalProgressDTO Proteins,
    GoalProgressDTO Fats,
    GoalProgressDTO Carbs,
    GoalProgressDTO Fiber,
    GoalProgressDTO Meals
);