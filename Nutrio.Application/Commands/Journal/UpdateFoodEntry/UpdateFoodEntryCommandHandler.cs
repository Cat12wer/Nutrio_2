using MediatR;
using Nutrio.Domain.Interfaces;
using Nutrio.Domain.ValueObjects;

namespace Nutrio.Application.Commands.Journal.UpdateFoodEntry;

public class UpdateFoodEntryCommandHandler : IRequestHandler<UpdateFoodEntryCommandDTO>
{
    private readonly IFoodEntryRepository _foodEntryRepository;

    public UpdateFoodEntryCommandHandler(IFoodEntryRepository foodEntryRepository)
    {
        _foodEntryRepository = foodEntryRepository;
    }

    public async Task Handle(UpdateFoodEntryCommandDTO request, CancellationToken cancellationToken)
    {
        // 1. Знаходимо запис у базі
        var entry = await _foodEntryRepository.GetByIdAsync(request.EntryId);

        // 2. Безпека: перевіряємо, чи існує запис і чи належить він поточному користувачу
        if (entry == null || entry.UserId != request.UserId)
            throw new UnauthorizedAccessException("Запис не знайдено або доступ заборонено.");

        // 3. Створюємо новий Value Object для ваги
        var newQuantity = new Quantity(request.Grams);

        // 4. Оновлюємо сутність через доменний метод
        entry.UpdateQuantity(newQuantity);

        // 5. Зберігаємо зміни
        _foodEntryRepository.Update(entry);
    }
}