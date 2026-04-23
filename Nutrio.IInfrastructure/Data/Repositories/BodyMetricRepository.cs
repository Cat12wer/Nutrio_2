using Microsoft.EntityFrameworkCore;
using Nutrio.Domain.Entities;
using Nutrio.Domain.Interfaces;

namespace Nutrio.Infrastructure.Persistence.Repositories;

public class BodyMetricRepository : Repository<BodyMetricStamp, Guid>, IBodyMetricRepository
{
    public BodyMetricRepository(NutrioDbContext context) : base(context) { }

    public async Task<BodyMetricStamp?> GetLatestByUserIdAsync(Guid userId)
    {
        return await _dbSet
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.DateOfEntry)
            .FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyList<BodyMetricStamp>> GetHistoryByUserIdAsync(Guid userId)
    {
        return await _dbSet
            .Where(b => b.UserId == userId)
            .OrderBy(b => b.DateOfEntry) // Сортування від найстарішого до найновішого
            .ToListAsync();
    }
}