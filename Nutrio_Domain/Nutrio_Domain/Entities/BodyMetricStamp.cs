using Nutrio.Domain.Common;
using Nutrio.Domain.ValueObjects;

namespace Nutrio.Domain.Entities;

public class BodyMetricStamp : Entity<Guid>
{
    public Guid UserId { get; private set; }
    public DateTime DateOfEntry { get; private set; }
    public BodyMetrics Metrics { get; private set; } // Weight, Height, Waist, Neck [cite: 113]
    public decimal? FatPercent { get; private set; }

    public BodyMetricStamp(Guid userId, DateTime dateOfEntry, BodyMetrics metrics, decimal? fatPercent = null)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        DateOfEntry = dateOfEntry;
        Metrics = metrics;
        FatPercent = fatPercent;
    }
}