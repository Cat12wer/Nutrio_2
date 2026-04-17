using FluentValidation;

namespace Nutrio.Application.Commands.Auth.Register;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Ім'я не може бути порожнім.")
            .MaximumLength(50).WithMessage("Ім'я занадто довге.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Прізвище не може бути порожнім.")
            .MaximumLength(50).WithMessage("Прізвище занадто довге.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email не може бути порожнім.")
            .EmailAddress().WithMessage("Некоректний формат Email.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Пароль не може бути порожнім.")
            .MinimumLength(8).WithMessage("Пароль повинен містити мінімум 8 символів.");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Дата народження є обов'язковою.")
            .LessThan(DateTime.Today).WithMessage("Дата народження не може бути в майбутньому.");

        RuleFor(x => x.Sex)
            .IsInEnum().WithMessage("Некоректне значення статі.");
    }
}