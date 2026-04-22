using MediatR;
using Nutrio.Domain.Interfaces;

namespace Nutrio.Application.Queries.Analytics.WeightDynamics;

public class GetWeightDynamicsQueryHandler : IRequestHandler<GetWeightDynamicsQuery, WeightDynamicsDTO>
{
    private readonly IUserRepository _userRepository;
    private readonly IBodyMetricRepository _bodyMetricRepository;

    public GetWeightDynamicsQueryHandler(
        IUserRepository userRepository,
        IBodyMetricRepository bodyMetricRepository)
    {
        _userRepository = userRepository;
        _bodyMetricRepository = bodyMetricRepository;
    }

    public async Task<WeightDynamicsDTO> Handle(GetWeightDynamicsQuery request, CancellationToken cancellationToken)
    {
        // 1. Отримуємо користувача, щоб дізнатися його цільову вагу (TargetWeight)
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user == null)
            throw new UnauthorizedAccessException("Користувача не знайдено.");

        // 2. Отримуємо всю історію замірів тіла
        var history = await _bodyMetricRepository.GetHistoryByUserIdAsync(request.UserId);

        // 3. Фільтруємо записи за вказаний період та сортуємо їх від найстарішого до найновішого
        var points = history
            .Where(h => h.DateOfEntry.Date >= request.StartDate.Date && h.DateOfEntry.Date <= request.EndDate.Date)
            .OrderBy(h => h.DateOfEntry) // Щоб лінія на графіку йшла зліва направо правильним шляхом
            .Select(h => new WeightPointDTO(
                h.DateOfEntry.Date,
                h.Metrics.Weight
            ))
            .ToList();

        // 4. Повертаємо готовий об'єкт для графіка
        return new WeightDynamicsDTO(user.TargetWeight, points);
    }
}