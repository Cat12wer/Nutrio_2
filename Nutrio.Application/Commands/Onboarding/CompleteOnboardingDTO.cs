using MediatR;
using Nutrio.Domain.Enums;

namespace Nutrio.Application.Commands.Onboarding; // Виправлений простір імен

// 1. DTO: те, що приходить з фронтенду (без UserId)
public record CompleteOnboardingDTO(
    string Name,
    string LastName,
    Sex Sex,
    DateTime DateOfBirth,
    int Height,
    decimal CurrentWeight,
    decimal TargetWeight,
    WeightGoal WeightGoal,
    ActivityLevel ActivityLevel
);

// 2. Command: те, що йде в MediatR (з UserId)
public record CompleteOnboardingCommand(
    Guid UserId,
    string Name,
    string LastName,
    Sex Sex,
    DateTime DateOfBirth,
    int Height,
    decimal CurrentWeight,
    decimal TargetWeight,
    WeightGoal WeightGoal,
    ActivityLevel ActivityLevel
) : IRequest<decimal>;