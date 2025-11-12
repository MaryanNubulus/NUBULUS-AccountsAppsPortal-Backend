namespace Nubulus.Backend.Api.Features.User;

public class CreateUserRequest
{
    public const string Route = "/api/v1/accounts/{accountId}/users";
    public int AccountId { get; set; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;

    public CreateUserRequest() { }

    private CreateUserRequest(int accountId, string name, string email)
    {
        AccountId = accountId;
        Name = name;
        Email = email;
    }

    public Dictionary<string, string[]> Validate()
    {
        var errors = new Dictionary<string, string[]>();

        // Validación AccountId
        if (AccountId <= 0)
            errors["AccountId"] = new[] { "AccountId must be a positive integer." };

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

        return errors;
    }

    public static CreateUserRequest Create(int accountId, string name, string email)
    {
        return new CreateUserRequest(accountId, name, email);
    }
}
