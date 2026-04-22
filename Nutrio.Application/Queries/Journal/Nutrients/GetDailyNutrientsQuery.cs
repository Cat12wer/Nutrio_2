// Nutrio.Application/Queries/Journal/Nutrients/GetDailyNutrientsQuery.cs
using MediatR;

namespace Nutrio.Application.Queries.Journal.Nutrients;

public record GetDailyNutrientsQuery(
    Guid UserId,
    DateTime Date
) : IRequest<DailyNutrientsSummaryDto>;