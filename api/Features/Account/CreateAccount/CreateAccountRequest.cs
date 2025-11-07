namespace Nubulus.Backend.Api.Features.Account;

public class CreateAccountRequest
{
    public const string Route = "/api/v1/accounts";
    public string Name { get; init; } = string.Empty;

    public string FullName { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string Phone { get; init; } = string.Empty;

    public string Address { get; init; } = string.Empty;

    public string NumberId { get; init; } = string.Empty;

    public CreateAccountRequest() { }

    private CreateAccountRequest(
        string name,
        string fullName,
        string email,
        string phone,
        string address,
        string numberId)
    {
        Name = name;
        FullName = fullName;
        Email = email;
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

        // Validación Email
        if (string.IsNullOrWhiteSpace(Email) || Email.Length < 5 || Email.Length > 100)
            errors["Email"] = new[] { "Email must be between 5 and 100 characters." };
        else
        {
            var emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            if (!System.Text.RegularExpressions.Regex.IsMatch(Email, emailPattern))
                errors["Email"] = new[] { "Invalid email format." };
        }

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

    public static CreateAccountRequest Create(
        string name,
        string fullName,
        string email,
        string phone,
        string address,
        string numberId)
    {
        return new CreateAccountRequest(
            name,
            fullName,
            email,
            phone,
            address,
            numberId);
    }
}
