// Nutrio.Application/Queries/Journal/Activity/GetWeeklyActivityQuery.cs
using MediatR;

namespace Nutrio.Application.Queries.Journal.Activity;

public record GetWeeklyActivityQuery(
    Guid UserId,
    DateTime CurrentDate // Будь-який день тижня, бекенд сам знайде понеділок і неділю
) : IRequest<WeeklyActivityDto>;
