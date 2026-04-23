using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nutrio.Domain.Entities;

namespace Nutrio.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // 1. Назва таблиці та первинний ключ
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);

        // 2. Базові властивості
        builder.Property(u => u.Name).IsRequired().HasMaxLength(50);
        builder.Property(u => u.LastName).IsRequired().HasMaxLength(50);
        builder.Property(u => u.HashPassword).IsRequired(); // Якщо буде GoogleAuth, то можна зробити не обов'язковим, але поки так
        builder.Property(u => u.TargetWeight).HasColumnType("decimal(5,2)");

        // 3. Мапінг Об'єкта-Значення (Value Object) - Email
        // OwnsOne каже EF Core розгорнути властивість Value об'єкта Email в окрему колонку таблиці Users
        builder.OwnsOne(u => u.Email, emailBuilder =>
        {
            emailBuilder.Property(e => e.Value)
                .HasColumnName("Email")
                .IsRequired()
                .HasMaxLength(150);

            // Робимо Email унікальним на рівні БД
            emailBuilder.HasIndex(e => e.Value).IsUnique();
        });

        // 4. Мапінг Enum (Зберігаємо як цілі числа)
        builder.Property(u => u.Sex).HasConversion<int>();
        builder.Property(u => u.ActivityLevel).HasConversion<int>();
        builder.Property(u => u.WeightGoal).HasConversion<int>();

        // 5. Зв'язки (Відношення 1 до багатьох)
        // Кажемо EF Core мапити закриті колекції
        builder.HasMany(u => u.FoodEntries)
            .WithOne()
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Cascade); // Якщо видаляємо юзера, видаляється його їжа

        builder.HasMany(u => u.MetricStamps)
            .WithOne()
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Дозволяємо EF Core записувати в наші private readonly колекції
        builder.Metadata.FindNavigation(nameof(User.FoodEntries))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Metadata.FindNavigation(nameof(User.MetricStamps))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}