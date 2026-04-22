using MediatR;
using Nutrio.Domain.Interfaces;

namespace Nutrio.Application.Commands.Journal.DeleteFoodEntry;

public class DeleteFoodEntryCommandHandler : IRequestHandler<DeleteFoodEntryCommand>
{
    private readonly IFoodEntryRepository _foodEntryRepository;

    public DeleteFoodEntryCommandHandler(IFoodEntryRepository foodEntryRepository)
    {
        _foodEntryRepository = foodEntryRepository;
    }

    public async Task Handle(DeleteFoodEntryCommand request, CancellationToken cancellationToken)
    {
        // 1. Шукаємо страву
        var entry = await _foodEntryRepository.GetByIdAsync(request.EntryId);

        // 2. Перевіряємо безпеку
        if (entry == null || entry.UserId != request.UserId)
            throw new UnauthorizedAccessException("Запис не знайдено або доступ заборонено.");

        // 3. Видаляємо через репозиторій
        _foodEntryRepository.Delete(entry);
    }
}