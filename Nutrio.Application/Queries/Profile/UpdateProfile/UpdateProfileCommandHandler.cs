using MediatR;
using Nutrio.Domain.Entities;
using Nutrio.Domain.Interfaces;
using Nutrio.Domain.ValueObjects;

namespace Nutrio.Application.Commands.Profile.UpdateProfile;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, decimal>
{
    private readonly IUserRepository _userRepository;
    private readonly IBodyMetricRepository _bodyMetricRepository;

    public UpdateProfileCommandHandler(
        IUserRepository userRepository,
        IBodyMetricRepository bodyMetricRepository)
    {
        _userRepository = userRepository;
        _bodyMetricRepository = bodyMetricRepository;
    }

    public async Task<decimal> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        // 1. Отримуємо користувача
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user == null)
            throw new UnauthorizedAccessException("Користувача не знайдено.");

        // 2. Оновлюємо цілі профілю
        user.UpdateProfile(request.TargetWeight, request.WeightGoal, request.ActivityLevel);
        _userRepository.Update(user);

        // 3. Отримуємо останні заміри, щоб дізнатися попередню вагу та незмінний зріст
        var latestMetrics = await _bodyMetricRepository.GetLatestByUserIdAsync(user.Id);
        if (latestMetrics == null)
            throw new InvalidOperationException("Метрики тіла не знайдені.");

        // 4. Якщо користувач ввів нову поточну вагу, створюємо новий запис в історію
        if (latestMetrics.Metrics.Weight != request.CurrentWeight)
        {
            var newMetrics = new BodyMetrics(request.CurrentWeight, latestMetrics.Metrics.Height);
            var newStamp = new BodyMetricStamp(user.Id, DateTime.UtcNow, newMetrics);
            await _bodyMetricRepository.AddAsync(newStamp);
        }

        // 5. Перераховуємо нову денну норму калорій на основі оновлених цілей та ваги
        // Використовуємо доменний метод, який реалізує формулу Міффліна-Сан Жеора
        decimal newDailyNorm = user.CalculateDailyCalorieNorm(request.CurrentWeight, latestMetrics.Metrics.Height);

        return newDailyNorm;
    }
}