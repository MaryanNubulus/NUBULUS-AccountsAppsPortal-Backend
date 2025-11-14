using System.Text.RegularExpressions;

namespace Nubulus.Domain.ValueObjects;

public class AppKey
{
    public string Value { get; private set; }

    public AppKey(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("App key cannot be null or empty.", nameof(value));

        if (value.Length < 3 || value.Length > 100)
            throw new ArgumentException("App key must be between 3 and 100 characters.", nameof(value));

        // Solo letras, números, guiones, sin espacios ni caracteres especiales
        var pattern = @"^[a-zA-Z0-9\-]+$";
        if (!Regex.IsMatch(value, pattern))
            throw new ArgumentException("App key can only contain letters, numbers, and hyphens.", nameof(value));

        Value = value;
    }

    public override string ToString() => Value;

    public override bool Equals(object? obj)
    {
        if (obj is AppKey other)
            return Value == other.Value;
        return false;
    }

    public override int GetHashCode() => Value.GetHashCode();
}
