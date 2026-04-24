using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using Nutrio.Application.Commands.Auth.Google; // Шлях до інтерфейсу та класу GoogleUserInfo

namespace Nutrio.Infrastructure.Authentication;

public class GoogleTokenValidator : IGoogleTokenValidator
{
    private readonly IConfiguration _configuration;

    public GoogleTokenValidator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<GoogleUserInfo?> ValidateAsync(string googleToken, CancellationToken cancellationToken = default)
    {
        try
        {
            // Отримуємо Client ID з налаштувань (рекомендовано для безпеки)
            var clientId = _configuration["GoogleAuth:ClientId"];

            var settings = new GoogleJsonWebSignature.ValidationSettings();

            // Якщо Client ID налаштовано, перевіряємо, чи токен видано саме для нашого додатку
            if (!string.IsNullOrEmpty(clientId))
            {
                settings.Audience = new[] { clientId };
            }

            // Валідуємо токен (перевіряємо підпис Google, термін дії тощо)
            var payload = await GoogleJsonWebSignature.ValidateAsync(googleToken, settings);

            if (payload == null)
                return null;

            // Мапимо дані від Google у нашу DTO
            return new GoogleUserInfo
            {
                GoogleId = payload.Subject, // Subject (sub) - це унікальний ID користувача в системі Google
                Email = payload.Email,
                FirstName = payload.GivenName ?? string.Empty,
                LastName = payload.FamilyName ?? string.Empty
            };
        }
        catch (InvalidJwtException)
        {
            // Якщо токен невалідний (прострочений, підроблений), бібліотека викидає цей виняток
            return null;
        }
    }
}