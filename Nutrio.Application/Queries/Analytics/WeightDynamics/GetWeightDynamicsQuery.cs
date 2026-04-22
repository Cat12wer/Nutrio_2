using MediatR;

namespace Nutrio.Application.Queries.Analytics.WeightDynamics;

public record GetWeightDynamicsQuery(
    Guid UserId,
    DateTime StartDate,
    DateTime EndDate
) : IRequest<WeightDynamicsDTO>;