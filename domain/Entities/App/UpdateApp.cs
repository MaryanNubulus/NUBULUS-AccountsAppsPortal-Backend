using Nubulus.Domain.ValueObjects;

namespace Nubulus.Domain.Entities.App;

public class UpdateApp
{
    public AppId AppId { get; private set; } = default!;
    public string Name { get; private set; } = default!;

    public UpdateApp(AppId appId, string name)
    {
        AppId = appId;
        Name = name;

        UpdateAppValidator validator = new UpdateAppValidator(this);
    }
}

internal sealed class UpdateAppValidator
{
    public UpdateAppValidator(UpdateApp command)
    {
        if (command == null)
            throw new ArgumentNullException(nameof(command), "UpdateApp command cannot be null.");

        // Validación AppId
        if (command.AppId.Value <= 0)
            throw new ArgumentException("App ID must be greater than 0.", nameof(command.AppId));

        // Validación Name
        if (string.IsNullOrWhiteSpace(command.Name) || command.Name.Length < 3 || command.Name.Length > 100)
            throw new ArgumentException("Name must be between 3 and 100 characters.", nameof(command.Name));
    }
}
