using Nubulus.Domain.ValueObjects;

namespace Nubulus.Domain.Entities.User;

public class UpdateUser
{
    public UserKey UserKey { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public EmailAddress Email { get; private set; } = default!;

    public UpdateUser(UserKey userKey, string name, EmailAddress email)
    {
        UserKey = userKey;
        Name = name;
        Email = email;

        UpdateUserValidator validator = new UpdateUserValidator(this);
    }
}

internal sealed class UpdateUserValidator
{
    public UpdateUserValidator(UpdateUser command)
    {
        if (command == null)
            throw new ArgumentNullException(nameof(command), "UpdateUser command cannot be null.");

        // Validación UserKey
        if (string.IsNullOrWhiteSpace(command.UserKey.Value))
            throw new ArgumentException("User key is required.", nameof(command.UserKey));
        if (command.UserKey.Value.Length > 36)
            throw new ArgumentException("User key must not exceed 36 characters.", nameof(command.UserKey));

        // Validación Name
        if (string.IsNullOrWhiteSpace(command.Name) || command.Name.Length < 2 || command.Name.Length > 100)
            throw new ArgumentException("Name must be between 2 and 100 characters.", nameof(command.Name));

        // Validación Email
        if (string.IsNullOrWhiteSpace(command.Email.Value) || command.Email.Value.Length < 5 || command.Email.Value.Length > 100)
            throw new ArgumentException("Email must be between 5 and 100 characters.", nameof(command.Email));
        else
        {
            var emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            if (!System.Text.RegularExpressions.Regex.IsMatch(command.Email.Value, emailPattern))
                throw new ArgumentException("Invalid email format.", nameof(command.Email));
        }
    }
}
