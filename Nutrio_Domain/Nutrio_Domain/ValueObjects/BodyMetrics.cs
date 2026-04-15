using Nutrio.Domain.Common;

namespace Nutrio.Domain.ValueObjects;

public class BodyMetrics : ValueObject
{
    public decimal Weight { get; }
    public int Height { get; }

    public BodyMetrics(decimal weight, int height)
    {
        // Виправляємо проблему з макету: не дозволяємо нульову вагу або зріст
        if (weight <= 0 || height <= 0)
            throw new InvalidBodyMetricsException("Вага та зріст мають бути більшими за нуль.");

        Weight = weight;
        Height = height;
    }

    // Розрахунок BMI для аналітики
    public decimal CalculateBmi()
    {
        var heightInMeters = Height / 100m;
        return Weight / (heightInMeters * heightInMeters);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Weight;
        yield return Height;
    }
}