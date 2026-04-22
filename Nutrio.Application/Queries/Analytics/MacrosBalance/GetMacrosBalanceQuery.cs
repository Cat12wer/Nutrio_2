using MediatR;

namespace Nutrio.Application.Queries.Analytics.MacrosBalance;

public record GetMacrosBalanceQuery(
    Guid UserId,
    DateTime StartDate,
    DateTime EndDate
) : IRequest<MacrosBalanceDTO>;