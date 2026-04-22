using MediatR;

namespace Nutrio.Application.Queries.Analytics.WeeklyGoalsProgress;

public record GetWeeklyGoalsProgressQuery(
    Guid UserId,
    DateTime StartDate,
    DateTime EndDate
) : IRequest<WeeklyGoalsProgressDTO>;