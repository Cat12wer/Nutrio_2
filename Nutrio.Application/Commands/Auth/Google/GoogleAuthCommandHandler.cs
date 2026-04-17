using MediatR;
using Nutrio.Application.Commands.Auth.Google;
using Nutrio.Application.DTOs;
using Nutrio.Application.Interfaces;
using Nutrio.Domain.Entities;
using Nutrio.Domain.Enums;
using Nutrio.Domain.Interfaces;
using Nutrio.Domain.ValueObjects;

namespace Nutrio.Application.Commands.Auth.GoogleAuth;

public class GoogleAuthCommandHandler : IRequestHandler<GoogleAuthCommand, AuthResultDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IGoogleTokenValidator _googleTokenValidator;

    public GoogleAuthCommandHandler(
        IUserRepository userRepository,
        IJwtTokenGenerator jwtTokenGenerator,
        IGoogleTokenValidator googleTokenValidator)
    {
        _userRepository = userRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
        _googleTokenValidator = googleTokenValidator;
    }

    public async Task<AuthResultDto> Handle(GoogleAuthCommand request, CancellationToken cancellationToken)
    {
        // 1. Перевіряємо Google Token та отримуємо дані користувача
        var googleUser = await _googleTokenValidator.ValidateAsync(request.GoogleToken, cancellationToken);
        if (googleUser == null)
        {
            throw new UnauthorizedAccessException("Недійсний токен Google.");
        }

        var email = new Email(googleUser.Email);

        // 2. Шукаємо користувача за GoogleId або за Email
        var user = await _userRepository.GetByGoogleIdAsync(googleUser.GoogleId)
                   ?? await _userRepository.GetByEmailAsync(email);

        // 3. Якщо користувача немає в базі, створюємо нового (Реєстрація)
        if (user == null)
        {
            // Створюємо нового користувача. 
            // Оскільки це вхід через Google, деякі дані (вік, стать) доведеться заповнити дефолтними значеннями, 
            // і запросити їх оновлення під час онбордингу на фронтенді.
            user = new User(
                googleUser.FirstName,
                googleUser.LastName,
                email,
                string.Empty, // Пароль не потрібен, якщо вхід через Google
                DateTime.MinValue, // Дефолтне значення
                Sex.Other // Дефолтне значення
            );

            // Примітка: Щоб це працювало ідеально, тобі потрібно буде додати властивість GoogleId у клас User 
            // та відповідний метод для її встановлення у доменному шарі.

            await _userRepository.AddAsync(user);
        }

        // 4. Генеруємо внутрішній JWT токен Nutrio
        var token = _jwtTokenGenerator.GenerateToken(user);

        // 5. Повертаємо результат на фронтенд
        return new AuthResultDto(
            user.Id,
            user.Name,
            user.LastName,
            user.Email.Value,
            token
        );
    }
}