using Nutrio.Domain.Exceptions;

public class InvalidEmailException : DomainException
{
    public InvalidEmailException(string email)
        : base($"Формат електронної пошти '{email}' є некоректним.") { }
}
