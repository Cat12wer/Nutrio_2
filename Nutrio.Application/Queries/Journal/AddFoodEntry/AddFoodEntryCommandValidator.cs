using FluentValidation;

namespace Nutrio.Application.Commands.Journal.AddFoodEntry;

public class AddFoodEntryCommandValidator : AbstractValidator<AddFoodEntryCommand>
{
    public AddFoodEntryCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.ProductId).GreaterThan(0);
        RuleFor(x => x.Date).NotEmpty();
        RuleFor(x => x.MealType).IsInEnum();

        // Перевірка згідно з нашою доменною логікою (не можна додати 0 грамів)
        RuleFor(x => x.Grams)
            .GreaterThan(0).WithMessage("Вага продукту повинна бути більшою за нуль.");
    }
}