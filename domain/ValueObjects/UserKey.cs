namespace Nubulus.Domain.ValueObjects;

public class UserKey
{
    public string Value { get; private set; } = default!;

    public UserKey(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("User key cannot be null or empty.", nameof(value));
        if (value.Length > 36)
            throw new ArgumentException("User key must not exceed 36 characters.", nameof(value));

        Value = value;
    }
}
