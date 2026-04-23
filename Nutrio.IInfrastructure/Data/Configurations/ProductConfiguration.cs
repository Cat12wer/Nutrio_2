using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nutrio.Domain.Entities;

namespace Nutrio.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");
        builder.HasKey(p => p.Id);

        // Назва продукту
        builder.Property(p => p.ProductName)
            .IsRequired()
            .HasMaxLength(200);

        // Мапінг Value Object - Nutrients
        builder.OwnsOne(p => p.NutrientsPer100g, nutrients =>
        {
            nutrients.Property(n => n.Calories).HasColumnName("Calories").HasColumnType("decimal(8,2)");
            nutrients.Property(n => n.Protein).HasColumnName("Protein").HasColumnType("decimal(8,2)");
            nutrients.Property(n => n.Fat).HasColumnName("Fat").HasColumnType("decimal(8,2)");
            nutrients.Property(n => n.Carbs).HasColumnName("Carbs").HasColumnType("decimal(8,2)");
            nutrients.Property(n => n.Fiber).HasColumnName("Fiber").HasColumnType("decimal(8,2)");
        });
    }
}