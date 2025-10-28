namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Common.ValueObjects;

public record Email
{
    public string Value { get; }

    public static Email Create(string value) => new Email(value);

    private Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !value.Contains("@"))
        {
            throw new ArgumentException("Invalid email format.", nameof(value));
        }
        if (value.Length > 254)
        {
            throw new ArgumentException("Email length exceeds maximum allowed length.", nameof(value));
        }

        Value = value;
    }
}