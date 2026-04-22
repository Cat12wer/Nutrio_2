using MediatR;

namespace Nutrio.Application.Queries.Analytics.MonthlyActivity;

// ДОДАЙ ": IRequest<MonthlyActivityDTO>" в кінці
public record GetMonthlyActivityQuery(
    Guid UserId,
    int Year,
    int Month
) : IRequest<MonthlyActivityDTO>;