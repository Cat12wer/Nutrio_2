// Nutrio.Application/Queries/Journal/Meals/GetDailyFoodEntriesQuery.cs
using MediatR;

namespace Nutrio.Application.Queries.Journal.Meals;

public record GetDailyFoodEntriesQuery(
    Guid UserId,
    DateTime Date
) : IRequest<List<MealGroupDto>>;