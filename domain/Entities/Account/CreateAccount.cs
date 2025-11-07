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
        {
            throw new ArgumentNullException(nameof(command), "CreateAccount command cannot be null.");
        }
        if (string.IsNullOrWhiteSpace(command.Key))
        {
            throw new ArgumentException("Account key is required.", nameof(command.Key));
        }
        if (command.Key.Length > 36)
        {
            throw new ArgumentException("Account key must not exceed 36 characters.", nameof(command.Key));
        }

        if (string.IsNullOrWhiteSpace(command.Name))
        {
            throw new ArgumentException("Account name is required.", nameof(command.Name));
        }
        if (command.Name.Length > 100)
        {
            throw new ArgumentException("Account name must not exceed 100 characters.", nameof(command.Name));
        }
        if (command.FullName.Length > 100)
        {
            throw new ArgumentException("Full name must not exceed 100 characters.", nameof(command.FullName));
        }
        if (command.Phone.Length > 15)
        {
            throw new ArgumentException("Phone number must not exceed 15 characters.", nameof(command.Phone));
        }
        if (command.Address.Length > 200)
        {
            throw new ArgumentException("Address must not exceed 200 characters.", nameof(command.Address));
        }
        if (command.NumberId.Length > 50)
        {
            throw new ArgumentException("Number ID must not exceed 50 characters.", nameof(command.NumberId));
        }
    }
}
