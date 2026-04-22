using MediatR;

namespace Nutrio.Application.Queries.Users.GetSidebarSummary;

public record GetUserSidebarSummaryQuery(
    Guid UserId
) : IRequest<SidebarSummaryDto>;