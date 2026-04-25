using MediatR;
using Nutrio.Domain.Enums;

namespace Nutrio.Application.Commands.Profile.UpdateProfile;

public record UpdateProfileCommand(
    Guid UserId,
    decimal CurrentWeight,
    decimal TargetWeight,
    WeightGoal WeightGoal,
    ActivityLevel ActivityLevel
) : IRequest<decimal>; // Повертаємо нову добову норму калорій