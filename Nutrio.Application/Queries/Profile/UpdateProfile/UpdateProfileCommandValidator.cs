using FluentValidation;

namespace Nutrio.Application.Commands.Profile.UpdateProfile;

public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();

        RuleFor(x => x.CurrentWeight)
            .GreaterThan(0).WithMessage("Поточна вага має бути більшою за нуль.");

        RuleFor(x => x.TargetWeight)
            .GreaterThan(0).WithMessage("Цільова вага має бути більшою за нуль.");

        RuleFor(x => x.WeightGoal).IsInEnum();
        RuleFor(x => x.ActivityLevel).IsInEnum();
    }
}