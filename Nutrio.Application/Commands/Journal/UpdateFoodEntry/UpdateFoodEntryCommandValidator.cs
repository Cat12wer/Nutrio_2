using FluentValidation;

namespace Nutrio.Application.Commands.Journal.UpdateFoodEntry;

public class UpdateFoodEntryCommandValidator : AbstractValidator<UpdateFoodEntryCommandDTO>
{
    public UpdateFoodEntryCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.EntryId).NotEmpty();

        // Знову перевіряємо бізнес-правило на рівні вхідних даних
        RuleFor(x => x.Grams)
            .GreaterThan(0).WithMessage("Вага продукту повинна бути більшою за нуль.");
    }
}