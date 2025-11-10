namespace Nubulus.Domain.ValueObjects;

public class AccountStatus
{
    public string Value { get; private set; }

    private AccountStatus(string value)
    {
        Value = value;
    }

    public static AccountStatus Active => new AccountStatus("Active");
    public static AccountStatus Inactive => new AccountStatus("Inactive");

    public static AccountStatus Parse(string status)
    {
        return status switch
        {
            "A" => Active,
            "I" => Inactive,
            _ => throw new ArgumentException($"Invalid account status: {status}")
        };
    }

}