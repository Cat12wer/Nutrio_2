using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nutrio.Application.Interfaces; // Додано для інтерфейсів Application
using Nutrio.Domain.Interfaces;
using Nutrio.Infrastructure.Authentication; // Додано для JwtTokenGenerator
using Nutrio.Infrastructure.Persistence;
using Nutrio.Infrastructure.Persistence.Repositories;
using Nutrio.Infrastructure.Security; // Додано для PasswordHasher

namespace Nutrio.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // 1. Реєструємо підключення до PostgreSQL
        services.AddDbContext<NutrioDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // 2. Реєструємо репозиторії
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IFoodEntryRepository, FoodEntryRepository>();
        services.AddScoped<IBodyMetricRepository, BodyMetricRepository>();

        // 3. Реєструємо сервіси Безпеки та Авторизації
        services.AddSingleton<IPasswordHasher, PasswordHasher>(); // Singleton, бо він не має стану (state)
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>(); // Scoped, бо використовує IConfiguration

        return services;
    }
}