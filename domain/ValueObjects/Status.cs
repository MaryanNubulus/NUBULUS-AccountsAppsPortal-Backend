namespace Nubulus.Domain.ValueObjects;

public class Status
{
    public string Value { get; private set; }

    private Status(string value)
    {
        Value = value;
    }

    public static Status Active => new Status("A");
    public static Status Inactive => new Status("I");

    public static Status Parse(string status)
    {
        return status switch
        {
            "A" => Active,
            "I" => Inactive,
            _ => throw new ArgumentException($"Invalid account status: {status}")
        };
    }


}