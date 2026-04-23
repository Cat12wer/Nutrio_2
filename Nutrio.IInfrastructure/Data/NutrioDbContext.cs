using Microsoft.EntityFrameworkCore;
using Nutrio.Domain.Entities;
using System.Reflection;

namespace Nutrio.Infrastructure.Persistence;

public class NutrioDbContext : DbContext
{
    public NutrioDbContext(DbContextOptions<NutrioDbContext> options) : base(options)
    {
    }

    // Наші таблиці
    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<FoodEntry> FoodEntries { get; set; }
    public DbSet<BodyMetricStamp> BodyMetricStamps { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Цей магічний рядок автоматично знайде всі класи конфігурацій 
        // (такі як UserConfiguration) у цій збірці і застосує їх!
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}