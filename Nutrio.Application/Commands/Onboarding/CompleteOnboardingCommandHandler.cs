using MediatR;
using Nutrio.Domain.Entities;
using Nutrio.Domain.Interfaces;
using Nutrio.Domain.ValueObjects;

// 1. Виправлено namespace відповідно до назви папки
namespace Nutrio.Application.Commands.Onboarding;

// 2. Виправлено тип: тепер він обробляє Command, а не DTO
public class CompleteOnboardingCommandHandler : IRequestHandler<CompleteOnboardingCommand, decimal>
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

    public async Task<decimal> Handle(CompleteOnboardingCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user == null)
            throw new UnauthorizedAccessException("Користувача не знайдено.");

        user.CompleteOnboarding(
          request.Name,
          request.LastName,
          request.Sex,
          request.DateOfBirth,
          request.TargetWeight,
          request.WeightGoal,
          request.ActivityLevel
        );
        _userRepository.Update(user);

        var initialMetrics = new BodyMetrics(request.CurrentWeight, request.Height);
        var metricStamp = new BodyMetricStamp(user.Id, DateTime.UtcNow, initialMetrics);
        await _bodyMetricRepository.AddAsync(metricStamp);

        var dailyCalorieNorm = user.CalculateDailyCalorieNorm(request.CurrentWeight, request.Height);
        return dailyCalorieNorm;
    }
}