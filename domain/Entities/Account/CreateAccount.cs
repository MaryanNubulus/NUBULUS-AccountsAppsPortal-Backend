using Nubulus.Domain.ValueObjects;

namespace Nubulus.Domain.Entities.Account;

public class CreateAccount
{
    public string Key { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public string FullName { get; private set; } = default!;
    public EmailAddress Email { get; private set; } = default!;
    public string Phone { get; private set; } = default!;
    public string Address { get; private set; } = default!;
    public string NumberId { get; private set; } = default!;

    public CreateAccount(string key, string name, string fullName, EmailAddress email, string phone, string address, string numberId)
    {
        Key = key;
        Name = name;
        FullName = fullName;
        Email = email;
        Phone = phone;
        Address = address;
        NumberId = numberId;

        CreateAccountValidator validator = new CreateAccountValidator(this);
    }
}

internal sealed class CreateAccountValidator
{
    public CreateAccountValidator(CreateAccount command)
    {
        if (command == null)
            throw new ArgumentNullException(nameof(command), "CreateAccount command cannot be null.");

        // Validación Key
        if (string.IsNullOrWhiteSpace(command.Key))
            throw new ArgumentException("Account key is required.", nameof(command.Key));
        if (command.Key.Length > 36)
            throw new ArgumentException("Account key must not exceed 36 characters.", nameof(command.Key));

        // Validación Name
        if (string.IsNullOrWhiteSpace(command.Name) || command.Name.Length < 2 || command.Name.Length > 100)
            throw new ArgumentException("Name must be between 2 and 100 characters.", nameof(command.Name));

        // Validación FullName
        if (!string.IsNullOrWhiteSpace(command.FullName) && command.FullName.Length > 100)
            throw new ArgumentException("Full name must not exceed 100 characters.", nameof(command.FullName));

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
        if (string.IsNullOrWhiteSpace(command.Phone) || command.Phone.Length < 10 || command.Phone.Length > 15)
            throw new ArgumentException("Phone must be between 10 and 15 characters.", nameof(command.Phone));
        else
        {
            var phonePattern = @"^\+?[0-9]{10,15}$";
            if (!System.Text.RegularExpressions.Regex.IsMatch(command.Phone, phonePattern))
                throw new ArgumentException("Invalid phone number format.", nameof(command.Phone));
        }

        // Validación Address
        if (string.IsNullOrWhiteSpace(command.Address) || command.Address.Length < 5 || command.Address.Length > 200)
            throw new ArgumentException("Address must be between 5 and 200 characters.", nameof(command.Address));

        // Validación NumberId
        if (string.IsNullOrWhiteSpace(command.NumberId) || command.NumberId.Length < 5 || command.NumberId.Length > 50)
            throw new ArgumentException("NumberId must be between 5 and 50 characters.", nameof(command.NumberId));
    }
}
