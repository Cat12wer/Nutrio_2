using MediatR;
using Nutrio.Application.DTOs;

namespace Nutrio.Application.Commands.Auth.Google;

public record GoogleAuthCommand(
    string GoogleToken
) : IRequest<AuthResultDto>;