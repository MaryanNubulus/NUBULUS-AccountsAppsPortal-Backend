namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Requests;

public record CreateAppRequest
{
    public string Key { get; init; }
    public string Name { get; init; }
    public CreateAppRequest()
    {
        Key = string.Empty;
        Name = string.Empty;
    }
    private CreateAppRequest(string key, string name)
    {
        Key = key;
        Name = name;
        Validate();
    }
    public CreateAppRequest Validate()
    {
        if (string.IsNullOrWhiteSpace(Key) || Key.Length > 50 || Key.Length < 5)
        {
            throw new ArgumentException("Invalid key.", nameof(Key));
        }
        if (string.IsNullOrWhiteSpace(Name) || Name.Length > 256 || Name.Length < 2)
        {
            throw new ArgumentException("Invalid name.", nameof(Name));
        }

        return this;
    }
    public static CreateAppRequest Create(string key, string name)
    {
        return new CreateAppRequest(key, name);
    }
}
