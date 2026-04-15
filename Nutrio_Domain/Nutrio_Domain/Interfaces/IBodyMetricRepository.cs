using Nutrio.Domain.Entities;

namespace Nutrio.Domain.Interfaces;

public interface IBodyMetricRepository : IRepository<BodyMetricStamp, Guid>
{
    Task<BodyMetricStamp?> GetLatestByUserIdAsync(Guid userId);
    Task<IReadOnlyList<BodyMetricStamp>> GetHistoryByUserIdAsync(Guid userId);
}