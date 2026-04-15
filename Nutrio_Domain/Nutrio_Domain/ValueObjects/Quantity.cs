using Nutrio.Domain.Common;
using Nutrio.Domain.Exceptions;

namespace Nutrio.Domain.ValueObjects;

public class Quantity : ValueObject
{
    public decimal Value { get; }
    public string Unit { get; } // Наприклад, "g" (грами)

    public Quantity(decimal value, string unit = "g")
    {
        // Наша валідація згідно з аналізом макету: не можна додати 0 грамів
        if (value <= 0)
            throw new FoodEntryValidationException("Вага продукту повинна бути більше нуля.");

        Value = value;
        Unit = unit;
    }

    // Метод для порівняння Value Objects
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
        yield return Unit;
    }
}