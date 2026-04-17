namespace Nutrio.Application.DTOs;

public record AuthResultDto(
    Guid UserId,
    string Name,
    string LastName,
    string Email,
    string Token
);