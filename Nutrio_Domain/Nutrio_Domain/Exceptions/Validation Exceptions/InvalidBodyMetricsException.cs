using Nutrio.Domain.Exceptions;

public class InvalidBodyMetricsException : DomainException
{
    public InvalidBodyMetricsException(string message)
        : base(message ?? "Показники тіла мають бути позитивними числами.") { }
}