using MediatR;
using Nutrio.Application.DTOs;
using Nutrio.Domain.Enums;

namespace Nutrio.Application.Commands.Auth.Register;

public record RegisterUserCommand(
    string Name,
    string LastName,
    string Email,
    string Password,
    DateTime DateOfBirth,
    Sex Sex
) : IRequest<AuthResultDto>;