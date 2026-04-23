using Nutrio.Application.Interfaces;
using BCrypt.Net;

namespace Nutrio.Infrastructure.Security;

public class PasswordHasher : IPasswordHasher
{
    public string Hash(string password)
    {
        // Генеруємо безпечний хеш за допомогою BCrypt
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool Verify(string password, string passwordHash)
    {
        // BCrypt сам розуміє сіль (salt) з хешу і перевіряє збіг
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}