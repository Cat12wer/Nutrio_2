using MediatR;
using Nutrio.Application.DTOs;

namespace Nutrio.Application.Commands.Auth.Google;

public record GoogleAuthDTO(
    string GoogleToken
) : IRequest<AuthResultDto>;