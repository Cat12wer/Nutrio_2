using Microsoft.EntityFrameworkCore;
using Nutrio.Domain.Entities;
using Nutrio.Domain.Interfaces;
using Nutrio.Domain.ValueObjects;

namespace Nutrio.Infrastructure.Persistence.Repositories;

public class UserRepository : Repository<User, Guid>, IUserRepository
{
    public UserRepository(NutrioDbContext context) : base(context) { }

    public async Task<User?> GetByEmailAsync(Email email)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Email.Value == email.Value);
    }

    public async Task<User?> GetByGoogleIdAsync(string googleId)
    {
        // Примітка: Поки що властивості GoogleId немає в доменній сутності User. 
        // Як тільки ти її додаси, розкоментуй цей рядок:
        // return await _dbSet.FirstOrDefaultAsync(u => u.GoogleId == googleId);
        return null;
    }

    public async Task<bool> IsEmailUniqueAsync(Email email)
    {
        // Повертаємо true, якщо в базі НЕМАЄ користувача з такою поштою
        return !await _dbSet.AnyAsync(u => u.Email.Value == email.Value);
    }
}