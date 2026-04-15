using Nutrio.Domain.Exceptions;

public class FoodEntryValidationException : DomainException
{
    public FoodEntryValidationException(string message) : base(message) { }
}