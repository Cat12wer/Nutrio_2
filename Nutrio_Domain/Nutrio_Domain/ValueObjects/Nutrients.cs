using Nutrio.Domain.Common;

namespace Nutrio.Domain.ValueObjects;

public class Nutrients : ValueObject
{
    public decimal Calories { get; }
    public decimal Protein { get; }
    public decimal Fat { get; }
    public decimal Carbs { get; }
    public decimal Fiber { get; }

    public Nutrients(decimal calories, decimal protein, decimal fat, decimal carbs, decimal fiber)
    {
        // Валідація: нутрієнти не можуть бути від'ємними (порада з аналізу макету)
        if (calories < 0 || protein < 0 || fat < 0 || carbs < 0 || fiber < 0)
            throw new DomainException("Показники поживних речовин не можуть бути від'ємними.");

        Calories = calories;
        Protein = protein;
        Fat = fat;
        Carbs = carbs;
        Fiber = fiber;
    }

    // Метод для розрахунку на основі грамів (використовується в FoodEntry)
    public Nutrients MultiplyByQuantity(decimal grams)
    {
        var factor = grams / 100m;
        return new Nutrients(
            Calories * factor,
            Protein * factor,
            Fat * factor,
            Carbs * factor,
            Fiber * factor);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Calories; yield return Protein; yield return Fat;
        yield return Carbs; yield return Fiber;
    }
}