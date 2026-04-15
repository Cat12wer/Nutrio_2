using System.Text.RegularExpressions;
using Nutrio.Domain.Common;
using Nutrio.Domain.Exceptions;

namespace Nutrio.Domain.ValueObjects;

public class Email : ValueObject
{
    private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
    public string Value { get; }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidEmailException("Email не може бути порожнім.");

        var normalized = value.Trim().ToLowerInvariant();

        if (!EmailRegex.IsMatch(normalized))
            throw new InvalidEmailException(value);

        Value = normalized;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}