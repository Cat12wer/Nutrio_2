using MediatR;
using Nutrio.Domain.Enums;

namespace Nutrio.Application.Commands.Journal.AddFoodEntry;

public record AddFoodEntryCommand(
    Guid UserId,          // З JWT токена
    int ProductId,        // З вибраного продукту у списку
    DateTime Date,        // Дата, вибрана в журналі
    MealType MealType,    // Тип прийому їжі (передається з віджета)
    decimal Grams         // Тимчасово фронтенд може слати 100
) : IRequest<Guid>;       // Повертаємо ID створеного запису