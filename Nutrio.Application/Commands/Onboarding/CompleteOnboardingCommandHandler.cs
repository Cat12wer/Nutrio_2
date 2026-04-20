using MediatR;
using Nutrio.Domain.Entities;
using Nutrio.Domain.Interfaces;
using Nutrio.Domain.ValueObjects;

namespace Nutrio.Application.Commands.Users.CompleteOnboarding;

public class CompleteOnboardingCommandHandler : IRequestHandler<CompleteOnboardingDTO, decimal>
{
    private readonly IUserRepository _userRepository;
    private readonly IBodyMetricRepository _bodyMetricRepository;

    public CompleteOnboardingCommandHandler(
        IUserRepository userRepository,
        IBodyMetricRepository bodyMetricRepository)
    {
        _userRepository = userRepository;
        _bodyMetricRepository = bodyMetricRepository;
    }

    public async Task<decimal> Handle(CompleteOnboardingDTO request, CancellationToken cancellationToken)
    {
        // 1. Отримуємо користувача з бази
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Користувача не знайдено.");
        }

        // 2. Оновлюємо дані користувача (додамо цей метод у домен нижче)
        user.CompleteOnboarding(
            request.Sex,
            request.DateOfBirth,
            request.TargetWeight,
            request.WeightGoal,
            request.ActivityLevel
        );

        _userRepository.Update(user);

        // 3. Створюємо перший запис про заміри тіла (BodyMetricStamp)
        var initialMetrics = new BodyMetrics(request.CurrentWeight, request.Height);
        var metricStamp = new BodyMetricStamp(user.Id, DateTime.UtcNow, initialMetrics);

        await _bodyMetricRepository.AddAsync(metricStamp);

        // 4. Розраховуємо денну норму калорій за допомогою нашого доменного методу
        var dailyCalorieNorm = user.CalculateDailyCalorieNorm(request.CurrentWeight, request.Height);

        // 5. Повертаємо норму калорій, щоб фронтенд міг її відобразити
        return dailyCalorieNorm;
    }
}