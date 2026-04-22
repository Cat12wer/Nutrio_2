using MediatR;

namespace Nutrio.Application.Commands.Journal.DeleteFoodEntry;

public record DeleteFoodEntryCommand(
    Guid UserId,
    Guid EntryId
) : IRequest;