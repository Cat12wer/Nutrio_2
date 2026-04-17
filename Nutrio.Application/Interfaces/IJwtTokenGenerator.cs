using Nutrio.Domain.Entities;

namespace Nutrio.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}