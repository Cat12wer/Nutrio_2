using MediatR;
namespace Nutrio.Application.Commands.Journal.UpdateFoodEntry;
public record UpdateFoodEntryCommand(
    Guid UserId,
    Guid EntryId,
    decimal Grams
) : IRequest;