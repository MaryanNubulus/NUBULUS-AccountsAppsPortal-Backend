namespace Nubulus.Backend.Api.Features.User;

public class UpdateUserRequest
{
    public const string Route = "/api/v1/accounts/{accountId}/users/{userId}";
    public int AccountId { get; set; }
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;

    public UpdateUserRequest() { }

    private UpdateUserRequest(int accountId, string fullName, string email, string phone)
    {
        AccountId = accountId;
        FullName = fullName;
        Email = email;
        Phone = phone;
    }

    public Dictionary<string, string[]> Validate()
    {
        var errors = new Dictionary<string, string[]>();

        // Validación AccountId
        if (AccountId <= 0)
            errors["AccountId"] = new[] { "AccountId must be a positive integer." };

        // Validación FullName
        if (string.IsNullOrWhiteSpace(FullName) || FullName.Length < 2 || FullName.Length > 100)
            errors["FullName"] = new[] { "Full name must be between 2 and 100 characters." };

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
        if (string.IsNullOrWhiteSpace(Phone) || Phone.Length > 15)
            errors["Phone"] = new[] { "Phone is required and must not exceed 15 characters." };

        return errors;
    }

    public static UpdateUserRequest Create(int accountId, string fullName, string email, string phone)
    {
        return new UpdateUserRequest(accountId, fullName, email, phone);
    }
}
