namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Requests;

public class UpdateAppRequest
{
    public string Name { get; init; }

    public UpdateAppRequest()
    {
        Name = string.Empty;
    }

    private UpdateAppRequest(string name)
    {
        Name = name;
        Validate();
    }
    public UpdateAppRequest Validate()
    {
        if (string.IsNullOrWhiteSpace(Name) || Name.Length > 256 || Name.Length < 2)
        {
            throw new ArgumentException("Invalid name.", nameof(Name));
        }

        return this;
    }
    public static UpdateAppRequest Create(string name)
    {
        return new UpdateAppRequest(name);
    }
}