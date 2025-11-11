namespace Nubulus.Domain.ValueObjects;

public class AccountId
{
    public int Value { get; private set; }
    public AccountId(int value)
    {
        if (value <= 0)
        {
            throw new ArgumentException("Account ID must be a positive integer.", nameof(value));
        }

        Value = value;
    }
}