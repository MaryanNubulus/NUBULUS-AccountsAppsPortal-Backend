using System.Text.RegularExpressions;

namespace Nubulus.Backend.Api.Features.App.CreateApp;

public class CreateAppRequest
{
    public const string Route = "/api/v1/apps";
    public string Key { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;

    public CreateAppRequest() { }

    private CreateAppRequest(string key, string name)
    {
        Key = key;
        Name = name;
        Validate();
    }

    public Dictionary<string, string[]> Validate()
    {
        var errors = new Dictionary<string, string[]>();

        // Validación Key
        if (string.IsNullOrWhiteSpace(Key) || Key.Length < 3 || Key.Length > 100)
            errors["Key"] = new[] { "Key must be between 3 and 100 characters." };
        else
        {
            // Solo letras, números, guiones, sin espacios ni caracteres especiales
            var keyPattern = @"^[a-zA-Z0-9\-]+$";
            if (!Regex.IsMatch(Key, keyPattern))
                errors["Key"] = new[] { "Key can only contain letters, numbers, and hyphens." };
        }

        // Validación Name
        if (string.IsNullOrWhiteSpace(Name) || Name.Length < 3 || Name.Length > 100)
            errors["Name"] = new[] { "Name must be between 3 and 100 characters." };

        return errors;
    }
}
