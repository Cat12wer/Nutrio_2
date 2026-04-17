using MediatR;
using Nutrio.Application.DTOs;

namespace Nutrio.Application.Commands.Auth.Login;

public record LoginUserCommand(
    string Email,
    string Password
) : IRequest<AuthResultDto>;