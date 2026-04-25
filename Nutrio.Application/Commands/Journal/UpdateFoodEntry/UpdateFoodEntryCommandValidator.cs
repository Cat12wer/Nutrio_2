using FluentValidation;

namespace Nutrio.Application.Commands.Journal.UpdateFoodEntry;

// Змінено тип
public class UpdateFoodEntryCommandValidator : AbstractValidator<UpdateFoodEntryCommand>
{
    public UpdateFoodEntryCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.EntryId).NotEmpty();
        RuleFor(x => x.Grams).GreaterThan(0).WithMessage("Вага продукту повинна бути більшою за нуль.");
    }
}