using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nutrio.Domain.Interfaces;
using Nutrio.Infrastructure.Persistence;
using Nutrio.Infrastructure.Persistence.Repositories;

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

        // 2. Реєструємо всі наші репозиторії
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IFoodEntryRepository, FoodEntryRepository>();
        services.AddScoped<IBodyMetricRepository, BodyMetricRepository>();

        // Тут пізніше ми також додамо JWT генератор та Password Hasher

        return services;
    }
}