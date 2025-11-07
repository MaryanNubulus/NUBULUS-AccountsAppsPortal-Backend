namespace Nubulus.Domain.ValueObjects;

public class AccountKey
{
    public string Value { get; private set; } = default!;

    public AccountKey(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Account key cannot be null or empty.", nameof(value));
        }
        if (value.Length > 36)
        {
            throw new ArgumentException("Account key must not exceed 36 characters.", nameof(value));
        }

        Value = value;
    }
}