using Nutrio.Domain.Enums;

namespace Nutrio.Application.Queries.Profile.GetUserProfile;

public record UserStatsDTO(
    int CurrentStreak,
    decimal WeightLostKg,
    int TotalEntries
);

public record UserGoalsDTO(
    decimal DailyCalories,
    decimal Proteins,
    decimal Fats,
    decimal Carbs,
    decimal WaterLiters // Поки що заглушка
);

public record UserBodyDTO(
    int Age,
    decimal CurrentWeight,
    decimal TargetWeight,
    WeightGoal Goal
);

// Головний об'єкт сторінки
public record UserProfileDTO(
    Guid Id,
    string Initials, // Наприклад "ІК" для Іван Коваль
    string FullName,
    string Email,
    UserStatsDTO Stats,
    UserGoalsDTO Goals,
    UserBodyDTO Body
);