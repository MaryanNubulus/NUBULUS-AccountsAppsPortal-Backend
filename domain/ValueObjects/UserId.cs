namespace Nubulus.Domain.ValueObjects;

public class UserId
{
    public int Value { get; private set; }
    public UserId(int value)
    {
        if (value <= 0)
        {
            throw new ArgumentException("User ID must be a positive integer.", nameof(value));
        }

        Value = value;
    }
}
