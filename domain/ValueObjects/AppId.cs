namespace Nubulus.Domain.ValueObjects;

public class AppId
{
    public int Value { get; private set; }

    public AppId(int value)
    {
        if (value <= 0)
            throw new ArgumentException("App ID must be greater than 0.", nameof(value));

        Value = value;
    }

    public override string ToString() => Value.ToString();

    public override bool Equals(object? obj)
    {
        if (obj is AppId other)
            return Value == other.Value;
        return false;
    }

    public override int GetHashCode() => Value.GetHashCode();
}
