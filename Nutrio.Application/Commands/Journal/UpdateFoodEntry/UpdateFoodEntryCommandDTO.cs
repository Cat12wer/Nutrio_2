using MediatR;

namespace Nutrio.Application.Commands.Journal.UpdateFoodEntry;

public record UpdateFoodEntryCommandDTO(
    Guid UserId,     // Для перевірки безпеки (щоб не редагували чужу їжу)
    Guid EntryId,    // ID запису, який редагуємо
    decimal Grams    // Нова вага
) : IRequest;        // Нічого не повертаємо (void)