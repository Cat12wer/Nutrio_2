using MediatR;
using Nutrio.Domain.Enums;

namespace Nutrio.Application.Commands.Users.CompleteOnboarding;

public record CompleteOnboardingDTO(
    Guid UserId,          // Отримується з JWT токена на рівні API контролера
    Sex Sex,
    DateTime DateOfBirth, // На макеті "Вік", але на бекенд краще передавати дату народження
    int Height,
    decimal CurrentWeight,
    decimal TargetWeight,
    WeightGoal WeightGoal,
    ActivityLevel ActivityLevel
) : IRequest<decimal>;