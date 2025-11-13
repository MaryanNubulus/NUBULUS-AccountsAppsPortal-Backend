using Nubulus.Domain.ValueObjects;

namespace Nubulus.Domain.Entities.User;

public class CreateUser
{
    public UserKey UserKey { get; private set; } = default!;
    public AccountKey ParentKey { get; private set; } = default!;
    public string FullName { get; private set; } = default!;
    public EmailAddress Email { get; private set; } = default!;
    public string Phone { get; private set; } = default!;
    public string? Password { get; private set; }

    public CreateUser(UserKey userKey, AccountKey parentKey, string fullName, EmailAddress email, string phone, string? password = null)
    {
        UserKey = userKey;
        ParentKey = parentKey;
        FullName = fullName;
        Email = email;
        Phone = phone;
        Password = password;

        CreateUserValidator validator = new CreateUserValidator(this);
    }
}

internal sealed class CreateUserValidator
{
    public CreateUserValidator(CreateUser command)
    {
        if (command == null)
            throw new ArgumentNullException(nameof(command), "CreateUser command cannot be null.");

        // Validación UserKey
        if (string.IsNullOrWhiteSpace(command.UserKey.Value))
            throw new ArgumentException("User key is required.", nameof(command.UserKey));
        if (command.UserKey.Value.Length > 36)
            throw new ArgumentException("User key must not exceed 36 characters.", nameof(command.UserKey));

        // Validación ParentKey
        if (string.IsNullOrWhiteSpace(command.ParentKey.Value))
            throw new ArgumentException("Parent key is required.", nameof(command.ParentKey));
        if (command.ParentKey.Value.Length > 36)
            throw new ArgumentException("Parent key must not exceed 36 characters.", nameof(command.ParentKey));

        // Validación FullName
        if (string.IsNullOrWhiteSpace(command.FullName) || command.FullName.Length < 2 || command.FullName.Length > 100)
            throw new ArgumentException("Full name must be between 2 and 100 characters.", nameof(command.FullName));

        // Validación Email
        if (string.IsNullOrWhiteSpace(command.Email.Value) || command.Email.Value.Length < 5 || command.Email.Value.Length > 100)
            throw new ArgumentException("Email must be between 5 and 100 characters.", nameof(command.Email));

        // Validación Phone
        if (string.IsNullOrWhiteSpace(command.Phone) || command.Phone.Length > 15)
            throw new ArgumentException("Phone is required and must not exceed 15 characters.", nameof(command.Phone));
        else
        {
            var emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            if (!System.Text.RegularExpressions.Regex.IsMatch(command.Email.Value, emailPattern))
                throw new ArgumentException("Invalid email format.", nameof(command.Email));
        }
    }
}
