namespace Nubulus.Backend.Api.Features.Account.UpdateAccount;

public class UpdateAccountRequest
{
    public const string Route = "/api/v1/accounts/{accountId}";
    public string Name { get; init; } = string.Empty;

    public string Phone { get; init; } = string.Empty;

    public string Address { get; init; } = string.Empty;

    public string NumberId { get; init; } = string.Empty;

    public UpdateAccountRequest() { }

    private UpdateAccountRequest(
        string name,
        string phone,
        string address,
        string numberId)
    {
        Name = name;
        Phone = phone;
        Address = address;
        NumberId = numberId;
    }

    public Dictionary<string, string[]> Validate()
    {
        var errors = new Dictionary<string, string[]>();

        // Validación Name
        if (string.IsNullOrWhiteSpace(Name) || Name.Length < 2 || Name.Length > 100)
            errors["Name"] = new[] { "Name must be between 2 and 100 characters." };


        // Validación Phone
        if (string.IsNullOrWhiteSpace(Phone) || Phone.Length < 10 || Phone.Length > 15)
            errors["Phone"] = new[] { "Phone must be between 10 and 15 characters." };
        else
        {
            var phonePattern = @"^\+?[0-9]{10,15}$";
            if (!System.Text.RegularExpressions.Regex.IsMatch(Phone, phonePattern))
                errors["Phone"] = new[] { "Invalid phone number format." };
        }

        // Validación Address
        if (string.IsNullOrWhiteSpace(Address) || Address.Length < 5 || Address.Length > 200)
            errors["Address"] = new[] { "Address must be between 5 and 200 characters." };

        // Validación NumberId
        if (string.IsNullOrWhiteSpace(NumberId) || NumberId.Length < 5 || NumberId.Length > 50)
            errors["NumberId"] = new[] { "NumberId must be between 5 and 50 characters." };

        return errors;
    }
}