using FluentValidation;

namespace Nutrio.Application.Commands.Users.CompleteOnboarding;

public class CompleteOnboardingCommandValidator : AbstractValidator<CompleteOnboardingDTO>
{
    public CompleteOnboardingCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();

        RuleFor(x => x.Sex).IsInEnum().WithMessage("Некоректно вказана стать.");

        RuleFor(x => x.DateOfBirth)
            .LessThan(DateTime.Today).WithMessage("Дата народження не може бути в майбутньому.");

        RuleFor(x => x.Height)
            .GreaterThan(0).WithMessage("Зріст має бути більшим за нуль.");

        RuleFor(x => x.CurrentWeight)
            .GreaterThan(0).WithMessage("Поточна вага має бути більшою за нуль.");

        RuleFor(x => x.TargetWeight)
            .GreaterThan(0).WithMessage("Цільова вага має бути більшою за нуль.");

        RuleFor(x => x.WeightGoal).IsInEnum();
        RuleFor(x => x.ActivityLevel).IsInEnum();
    }
}