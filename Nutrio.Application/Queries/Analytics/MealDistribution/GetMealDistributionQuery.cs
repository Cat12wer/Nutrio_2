using MediatR;

namespace Nutrio.Application.Queries.Analytics.MealDistribution;

public record GetMealDistributionQuery(
    Guid UserId,
    DateTime StartDate,
    DateTime EndDate
) : IRequest<MealDistributionDTO>;