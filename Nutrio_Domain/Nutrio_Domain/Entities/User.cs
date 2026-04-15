using Nutrio.Domain.Common;
using Nutrio.Domain.ValueObjects;
using Nutrio.Domain.Enums;

namespace Nutrio.Domain.Entities;

public class User : Entity<Guid>
{
    public string Name { get; private set; }
    public string LastName { get; private set; }
    public Email Email { get; private set; }
    public string HashPassword { get; private set; }
    public DateTime DateOfBirth { get; private set; }
    public Sex Sex { get; private set; }

    // Колекції для зв'язків
    private readonly List<FoodEntry> _foodEntries = new();
    public IReadOnlyCollection<FoodEntry> FoodEntries => _foodEntries.AsReadOnly();

    private readonly List<BodyMetricStamp> _metricStamps = new();
    public IReadOnlyCollection<BodyMetricStamp> MetricStamps => _metricStamps.AsReadOnly();

    // Конструктор для створення нового користувача
    public User(string name, string lastName, Email email, string hashPassword, DateTime dateOfBirth, Sex sex)
    {
        Id = Guid.NewGuid(); // Генеруємо Guid для безпеки [cite: 145]
        Name = name;
        LastName = lastName;
        Email = email;
        HashPassword = hashPassword;
        DateOfBirth = dateOfBirth;
        Sex = sex;
    }

    // Доменна логіка: Розрахунок віку для TDEE [cite: 30]
    public int GetAge()
    {
        var today = DateTime.Today;
        var age = today.Year - DateOfBirth.Year;
        if (DateOfBirth.Date > today.AddYears(-age)) age--;
        return age;
    }
}