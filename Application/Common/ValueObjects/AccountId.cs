namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Repositories;

public record AccountId
{
    public Guid Value { get; }

    public static AccountId Create(string value) => new AccountId(value);

    private AccountId(string value)
    {
        if (!Guid.TryParse(value, out var guidValue) || guidValue == Guid.Empty)
        {
            throw new ArgumentException("Id cannot be an empty GUID.", nameof(value));
        }

        Value = guidValue;
    }

    public override string ToString() => Value.ToString();
}
