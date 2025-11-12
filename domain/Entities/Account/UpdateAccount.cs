using Nubulus.Domain.ValueObjects;

namespace Nubulus.Domain.Entities.Account;

public class UpdateAccount
{
    public AccountId AccountId { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public string Phone { get; private set; } = default!;
    public string Address { get; private set; } = default!;
    public string NumberId { get; private set; } = default!;

    public UpdateAccount(AccountId accountId, string name, string phone, string address, string numberId)
    {
        AccountId = accountId;
        Name = name;
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
        if (command.AccountId.Value <= 0)
            throw new ArgumentException("Invalid account ID.", nameof(command.AccountId));

        // Validación Name
        if (string.IsNullOrWhiteSpace(command.Name) || command.Name.Length < 2 || command.Name.Length > 100)
            throw new ArgumentException("Name must be between 2 and 100 characters.", nameof(command.Name));

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