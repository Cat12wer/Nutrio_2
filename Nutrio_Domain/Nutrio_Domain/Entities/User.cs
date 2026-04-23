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
    public decimal TargetWeight { get; private set; }
    public Sex Sex { get; private set; }
    public ActivityLevel ActivityLevel { get; private set; }
    public WeightGoal WeightGoal { get; private set; }

    public string? GoogleId { get; private set; }

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

    public decimal CalculateDailyCalorieNorm(decimal currentWeight, int currentHeight)
    {
        // 1. Отримуємо вік за допомогою існуючого методу
        int age = GetAge();

        // 2. Розрахунок базового метаболізму (BMR) за формулою Міффліна-Сан Жеора
        // Згідно з твоїм запитом, Sex.Other використовує жіночу формулу (-161)
        decimal bmr = (10m * currentWeight) + (6.25m * currentHeight) - (5m * age);
        bmr += Sex == Sex.Male ? 5m : -161m;

        // 3. Застосування коефіцієнта активності (розрахунок TDEE)
        decimal tdee = ActivityLevel switch
        {
            ActivityLevel.None => bmr * 1.2m,     // Сидячий спосіб
            ActivityLevel.Low => bmr * 1.375m,    // Легка активність
            ActivityLevel.Middle => bmr * 1.55m,  // Помірна активність
            ActivityLevel.High => bmr * 1.725m,   // Висока активність
            _ => bmr * 1.2m                       // Дефолтне значення
        };

        // 4. Коригування калорій залежно від цілі
        decimal dailyNorm = WeightGoal switch
        {
            WeightGoal.Lose => tdee - 500m,       // Дефіцит для схуднення (~0.5 кг/тиждень)
            WeightGoal.Gain => tdee + 500m,       // Профіцит для набору маси
            WeightGoal.Maintain => tdee,          // Підтримка ваги
            _ => tdee
        };

        // 5. Медичний запобіжник: норма не повинна падати нижче критичного мінімуму
        if (dailyNorm < 1200m)
        {
            dailyNorm = 1200m;
        }

        return Math.Round(dailyNorm);
    }

    // Метод для оновлення профілю після онбордингу
    public void CompleteOnboarding(
        string Name,
        string LastName,
        Sex sex, 
        DateTime dateOfBirth, 
        decimal targetWeight, 
        WeightGoal weightGoal, 
        ActivityLevel activityLevel)
    {
        Sex = sex;
        DateOfBirth = dateOfBirth;
        TargetWeight = targetWeight;
        WeightGoal = weightGoal;
        ActivityLevel = activityLevel;
    }

    // Метод для редагування профілю з налаштувань
    public void UpdateProfile(decimal targetWeight, WeightGoal weightGoal, ActivityLevel activityLevel)
    {
        TargetWeight = targetWeight;
        WeightGoal = weightGoal;
        ActivityLevel = activityLevel;
    }

    public User(string name, string lastName, Email email, string googleId)
    {
        Id = Guid.NewGuid();
        Name = name;
        LastName = lastName;
        Email = email;
        GoogleId = googleId;
        HashPassword = string.Empty; // Для Google пароль не потрібен
        DateOfBirth = DateTime.MinValue;
        Sex = Sex.Other;
    }

    private User() { }
}