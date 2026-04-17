using MediatR;
using Nutrio.Application.DTOs;
using Nutrio.Application.Interfaces;
using Nutrio.Domain.Interfaces;
using Nutrio.Domain.ValueObjects;

namespace Nutrio.Application.Commands.Auth.Login;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, AuthResultDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<AuthResultDto> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        // 1. Формуємо доменний об'єкт Email
        var email = new Email(request.Email);

        // 2. Шукаємо користувача в базі за Email
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
        {
            // З міркувань безпеки ми не повідомляємо, що саме неправильно (email чи пароль)
            throw new UnauthorizedAccessException("Невірний email або пароль.");
        }

        // 3. Перевіряємо, чи збігається хеш пароля
        var isPasswordValid = _passwordHasher.Verify(request.Password, user.HashPassword);
        if (!isPasswordValid)
        {
            throw new UnauthorizedAccessException("Невірний email або пароль.");
        }

        // 4. Генеруємо JWT токен
        var token = _jwtTokenGenerator.GenerateToken(user);

        // 5. Повертаємо результат на фронтенд
        return new AuthResultDto(
            user.Id,
            user.Name,
            user.LastName,
            user.Email.Value, // Дістаємо string з об'єкта-значення Email
            token
        );
    }
}