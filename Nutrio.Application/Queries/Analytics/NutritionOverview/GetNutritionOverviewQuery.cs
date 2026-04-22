// Nutrio.Application/Queries/Analytics/NutritionOverview/GetNutritionOverviewQuery.cs
using MediatR;

namespace Nutrio.Application.Queries.Analytics.NutritionOverview;

public record GetNutritionOverviewQuery(
    Guid UserId,
    DateTime StartDate,
    DateTime EndDate
) : IRequest<NutritionOverviewDto>;