using MediatR;

namespace Nutrio.Application.Queries.Analytics.CaloriesChart;

public record GetCaloriesChartQuery(
    Guid UserId,
    DateTime StartDate,
    DateTime EndDate
) : IRequest<CaloriesChartDTO>;