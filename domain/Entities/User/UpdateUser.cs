using Nubulus.Domain.ValueObjects;

namespace Nubulus.Domain.Entities.User;

public class UpdateUser
{
    public UserId UserId { get; private set; } = default!;
    public string FullName { get; private set; } = default!;
    public EmailAddress Email { get; private set; } = default!;
    public string Phone { get; private set; } = default!;

    public UpdateUser(UserId userId, string fullName, EmailAddress email, string phone)
    {
        UserId = userId;
        FullName = fullName;
        Email = email;
        Phone = phone;

        UpdateUserValidator validator = new UpdateUserValidator(this);
    }
}

internal sealed class UpdateUserValidator
{
    public UpdateUserValidator(UpdateUser command)
    {
        if (command == null)
            throw new ArgumentNullException(nameof(command), "UpdateUser command cannot be null.");

        // Validación UserId
        if (command.UserId.Value <= 0)
            throw new ArgumentException("User ID must be greater than 0.", nameof(command.UserId));

        // Validación FullName
        if (string.IsNullOrWhiteSpace(command.FullName) || command.FullName.Length < 2 || command.FullName.Length > 100)
            throw new ArgumentException("Full name must be between 2 and 100 characters.", nameof(command.FullName));

        // Validación Email
        if (string.IsNullOrWhiteSpace(command.Email.Value) || command.Email.Value.Length < 5 || command.Email.Value.Length > 100)
            throw new ArgumentException("Email must be between 5 and 100 characters.", nameof(command.Email));
        else
        {
            var emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            if (!System.Text.RegularExpressions.Regex.IsMatch(command.Email.Value, emailPattern))
                throw new ArgumentException("Invalid email format.", nameof(command.Email));
        }

        // Validación Phone
        if (string.IsNullOrWhiteSpace(command.Phone) || command.Phone.Length > 15)
            throw new ArgumentException("Phone is required and must not exceed 15 characters.", nameof(command.Phone));
    }
}
