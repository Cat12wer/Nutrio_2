using MediatR;
using Nutrio.Application.DTOs;
using Nutrio.Application.Interfaces;
using Nutrio.Domain.Entities;
using Nutrio.Domain.Interfaces;
using Nutrio.Domain.ValueObjects;

namespace Nutrio.Application.Commands.Auth.Register;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, AuthResultDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<AuthResultDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        // 1. Створюємо доменний об'єкт Email (тут спрацює доменна валідація формату)
        var email = new Email(request.Email);

        // 2. Перевіряємо, чи вільна електронна пошта
        var isEmailUnique = await _userRepository.IsEmailUniqueAsync(email);
        if (!isEmailUnique)
        {
            // В ідеалі тут варто викинути специфічне Conflict Exception, про яке ми говорили (напр. EmailAlreadyInUseException)
            throw new InvalidOperationException("Користувач з таким Email вже існує.");
        }

        // 3. Хешуємо пароль для безпечного збереження
        var hashedPassword = _passwordHasher.Hash(request.Password);

        // 4. Створюємо нову сутність користувача
        var user = new User(
            request.Name,
            request.LastName,
            email,
            hashedPassword,
            request.DateOfBirth,
            request.Sex
        );

        // 5. Зберігаємо користувача в базу даних
        await _userRepository.AddAsync(user);

        // 6. Генеруємо JWT токен для успішної авторизації
        var token = _jwtTokenGenerator.GenerateToken(user);

        // 7. Повертаємо об'єкт з даними для фронтенду
        return new AuthResultDto(
            user.Id,
            user.Name,
            user.LastName,
            user.Email.Value, // Звертаємося до .Value, оскільки Email — це Value Object
            token
        );
    }
}