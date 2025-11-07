namespace Nubulus.Backend.Api.Features.Account;

public class CreateAccountRequest
{
    public string Name { get; init; }

    public string FullName { get; init; }

    public string Email { get; init; }

    public string Phone { get; init; }

    public string Address { get; init; }

    public string NumberId { get; init; }

    public CreateAccountRequest()
    {
        Name = string.Empty;
        FullName = string.Empty;
        Email = string.Empty;
        Phone = string.Empty;
        Address = string.Empty;
        NumberId = string.Empty;
    }

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
        Validate();
    }

    public CreateAccountRequest Validate()
    {
        if (string.IsNullOrWhiteSpace(Name) || Name.Length < 2 || Name.Length > 100)
            throw new ArgumentException("Name must be between 2 and 100 characters.");

        if (string.IsNullOrWhiteSpace(Email) || Email.Length < 5 || Email.Length > 100)
            throw new ArgumentException("Email must be between 5 and 100 characters.");

        var emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        if (!System.Text.RegularExpressions.Regex.IsMatch(Email, emailPattern))
            throw new ArgumentException("Invalid email format.");

        if (string.IsNullOrWhiteSpace(Phone) || Phone.Length < 10 || Phone.Length > 15)
            throw new ArgumentException("Phone must be between 10 and 15 characters.");

        var phonePattern = @"^\+?[0-9]{10,15}$";
        if (!System.Text.RegularExpressions.Regex.IsMatch(Phone, phonePattern))
            throw new ArgumentException("Invalid phone number format.");

        if (string.IsNullOrWhiteSpace(Address) || Address.Length < 5 || Address.Length > 200)
            throw new ArgumentException("Address must be between 5 and 200 characters.");

        if (string.IsNullOrWhiteSpace(NumberId) || NumberId.Length < 5 || NumberId.Length > 50)
            throw new ArgumentException("NumberId must be between 5 and 50 characters.");

        return this;
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
