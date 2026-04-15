namespace Nutrio.Domain.Exceptions;

public class InvalidNutrientsException : DomainException
{
    public InvalidNutrientsException(string message) : base(message) { }
}