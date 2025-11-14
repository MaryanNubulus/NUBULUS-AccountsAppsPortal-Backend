using System.Text.RegularExpressions;

namespace Nubulus.Backend.Api.Features.App.UpdateApp;

public class UpdateAppRequest
{
    public const string Route = "/api/v1/apps/{id}";
    public string Name { get; init; } = string.Empty;

    public UpdateAppRequest() { }

    private UpdateAppRequest(string name)
    {
        Name = name;
    }

    public Dictionary<string, string[]> Validate()
    {
        var errors = new Dictionary<string, string[]>();

        // Validación Name
        if (string.IsNullOrWhiteSpace(Name) || Name.Length < 3 || Name.Length > 100)
            errors["Name"] = new[] { "Name must be between 3 and 100 characters." };

        return errors;
    }
}
