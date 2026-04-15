using Nutrio.Domain.Entities;
using Nutrio.Domain.ValueObjects;

namespace Nutrio.Domain.Interfaces;

public interface IUserRepository : IRepository<User, Guid>
{
    Task<User?> GetByEmailAsync(Email email);
    Task<User?> GetByGoogleIdAsync(string googleId);
    Task<bool> IsEmailUniqueAsync(Email email);
}