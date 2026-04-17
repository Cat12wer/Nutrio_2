namespace Nutrio.Application.Commands.Auth.Google;

public class GoogleUserInfo
{
    public string GoogleId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}

public interface IGoogleTokenValidator
{
    // Метод приймає токен і повертає базові дані профілю, якщо токен валідний
    Task<GoogleUserInfo?> ValidateAsync(string googleToken, CancellationToken cancellationToken = default);
}