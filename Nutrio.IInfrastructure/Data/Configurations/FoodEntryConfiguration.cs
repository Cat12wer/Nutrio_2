using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nutrio.Domain.Entities;

namespace Nutrio.Infrastructure.Persistence.Configurations;

public class FoodEntryConfiguration : IEntityTypeConfiguration<FoodEntry>
{
    public void Configure(EntityTypeBuilder<FoodEntry> builder)
    {
        builder.ToTable("FoodEntries");
        builder.HasKey(f => f.Id);

        builder.Property(f => f.Date).IsRequired();

        // Зберігаємо Enum як число
        builder.Property(f => f.MealType).HasConversion<int>();

        // Мапінг Value Object - Quantity
        builder.OwnsOne(f => f.Quantity, quantity =>
        {
            quantity.Property(q => q.Value).HasColumnName("Grams").HasColumnType("decimal(8,2)");
            quantity.Property(q => q.Unit).HasColumnName("Unit").HasMaxLength(20);
        });

        // Налаштування зв'язку 1-до-багатьох із таблицею Products
        builder.HasOne(f => f.Product)
            .WithMany()
            .HasForeignKey(f => f.ProductId)
            .OnDelete(DeleteBehavior.Restrict); // Забороняємо видаляти продукт, якщо він є в чиємусь журналі
    }
}