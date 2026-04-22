// Nutrio.Application/Queries/Journal/Meals/DailyMealsDto.cs
using Nutrio.Domain.Enums;

namespace Nutrio.Application.Queries.Journal.Meals;

// Окремий продукт у прийомі їжі
public record FoodItemDto(
    Guid EntryId,
    string ProductName,
    decimal Grams,
    decimal Calories
);

// Група прийому їжі (Сніданок, Обід тощо)
public record MealGroupDto(
    MealType MealType,
    decimal TotalCalories,
    List<FoodItemDto> Items
);