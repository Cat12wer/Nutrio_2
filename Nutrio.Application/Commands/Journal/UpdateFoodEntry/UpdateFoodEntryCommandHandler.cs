using MediatR;
using Nutrio.Domain.Interfaces;
using Nutrio.Domain.ValueObjects;

namespace Nutrio.Application.Commands.Journal.UpdateFoodEntry;

// Змінено інтерфейс
public class UpdateFoodEntryCommandHandler : IRequestHandler<UpdateFoodEntryCommand>
{
    private readonly IFoodEntryRepository _foodEntryRepository;

    public UpdateFoodEntryCommandHandler(IFoodEntryRepository foodEntryRepository)
    {
        _foodEntryRepository = foodEntryRepository;
    }

    // Змінено тип request
    public async Task Handle(UpdateFoodEntryCommand request, CancellationToken cancellationToken)
    {
        var entry = await _foodEntryRepository.GetByIdAsync(request.EntryId);

        if (entry == null || entry.UserId != request.UserId)
            throw new UnauthorizedAccessException("Запис не знайдено або доступ заборонено.");

        var newQuantity = new Quantity(request.Grams);
        entry.UpdateQuantity(newQuantity);

        _foodEntryRepository.Update(entry);
    }
}