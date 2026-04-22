using FluentValidation;

namespace Nutrio.Application.Commands.Journal.DeleteFoodEntry;

public class DeleteFoodEntryCommandValidator : AbstractValidator<DeleteFoodEntryCommand>
{
    public DeleteFoodEntryCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.EntryId).NotEmpty();
    }
}