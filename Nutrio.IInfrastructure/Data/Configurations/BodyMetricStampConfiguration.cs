using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nutrio.Domain.Entities;

namespace Nutrio.Infrastructure.Persistence.Configurations;

public class BodyMetricStampConfiguration : IEntityTypeConfiguration<BodyMetricStamp>
{
    public void Configure(EntityTypeBuilder<BodyMetricStamp> builder)
    {
        builder.ToTable("BodyMetricStamps");
        builder.HasKey(b => b.Id);

        builder.Property(b => b.DateOfEntry).IsRequired();
        builder.Property(b => b.FatPercent).HasColumnType("decimal(5,2)").IsRequired(false);

        // Мапінг Value Object - BodyMetrics
        builder.OwnsOne(b => b.Metrics, metrics =>
        {
            metrics.Property(m => m.Weight).HasColumnName("Weight").HasColumnType("decimal(5,2)");
            metrics.Property(m => m.Height).HasColumnName("Height"); // int зберігається як int
        });
    }
}