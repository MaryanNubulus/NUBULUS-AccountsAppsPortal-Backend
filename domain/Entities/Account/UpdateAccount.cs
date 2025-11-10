using Nubulus.Domain.ValueObjects;

namespace Nubulus.Domain.Entities.Account;

public class UpdateAccount
{
    public int Id { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public EmailAddress Email { get; private set; } = default!;
    public string Phone { get; private set; } = default!;
    public string Address { get; private set; } = default!;
    public string NumberId { get; private set; } = default!;

    public UpdateAccount(int id, string name, EmailAddress email, string phone, string address, string numberId)
    {
        Id = id;
        Name = name;
        Email = email;
        Phone = phone;
        Address = address;
        NumberId = numberId;

        UpdateAccountValidator validator = new UpdateAccountValidator(this);
    }
}

internal sealed class UpdateAccountValidator
{
    public UpdateAccountValidator(UpdateAccount command)
    {
        if (command == null)
            throw new ArgumentNullException(nameof(command), "UpdateAccount command cannot be null.");

        // Validación Id
        if (command.Id <= 0)
            throw new ArgumentException("Invalid account ID.", nameof(command.Id));

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
        // Validación Phone 
        if (string.IsNullOrWhiteSpace(command.Phone) || command.Phone.Length < 10 || command.Phone.Length > 15)
            throw new ArgumentException("Phone must be between 10 and 15 characters.", nameof(command.Phone));
        else
        {
            var phonePattern = @"^\+?[0-9]{10,15}$";
            if (!System.Text.RegularExpressions.Regex.IsMatch(command.Phone, phonePattern))
                throw new ArgumentException("Invalid phone format.", nameof(command.Phone));
        }
        // Validación Address
        if (!string.IsNullOrWhiteSpace(command.Address) && command.Address.Length > 200)
            throw new ArgumentException("Address must not exceed 200 characters.", nameof(command.Address));
        // Validación NumberId
        if (!string.IsNullOrWhiteSpace(command.NumberId) && command.NumberId.Length > 50)
            throw new ArgumentException("NumberId must not exceed 50 characters.", nameof(command.NumberId));
    }
}