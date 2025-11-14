using Nubulus.Domain.ValueObjects;

namespace Nubulus.Domain.Entities.App;

public class CreateApp
{
    public AppKey AppKey { get; private set; } = default!;
    public string Name { get; private set; } = default!;

    public CreateApp(AppKey appKey, string name)
    {
        AppKey = appKey;
        Name = name;

        CreateAppValidator validator = new CreateAppValidator(this);
    }
}

internal sealed class CreateAppValidator
{
    public CreateAppValidator(CreateApp command)
    {
        if (command == null)
            throw new ArgumentNullException(nameof(command), "CreateApp command cannot be null.");

        // Validación AppKey
        if (string.IsNullOrWhiteSpace(command.AppKey.Value))
            throw new ArgumentException("App key is required.", nameof(command.AppKey));
        if (command.AppKey.Value.Length < 3 || command.AppKey.Value.Length > 100)
            throw new ArgumentException("App key must be between 3 and 100 characters.", nameof(command.AppKey));

        // Validación Name
        if (string.IsNullOrWhiteSpace(command.Name) || command.Name.Length < 3 || command.Name.Length > 100)
            throw new ArgumentException("Name must be between 3 and 100 characters.", nameof(command.Name));
    }
}
