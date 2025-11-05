using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.ValueObjects;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Requests;

public record CreateUserRequest
{
    public string AccountId { get; init; }
    public string Name { get; init; }
    public string Email { get; init; }
    public string Phone { get; init; }
    public string Role { get; init; }

    public CreateUserRequest()
    {
        AccountId = string.Empty;
        Name = string.Empty;
        Email = string.Empty;
        Phone = string.Empty;
        Role = string.Empty;
    }

    private CreateUserRequest(string accountId, string name, string email, string phone, string role)
    {
        if (!Guid.TryParse(accountId, out var accountGuid))
        {
            throw new ArgumentException("Invalid account ID format.", nameof(accountId));
        }
        AccountId = IdObject.ValidateId(accountGuid).ToString();
        Name = name;
        Email = NUBULUS.AccountsAppsPortalBackEnd.Application.Common.ValueObjects.Email.ValidateEmail(email);
        Phone = phone;
        Role = role;
        Validate();
    }

    public CreateUserRequest Validate()
    {
        if (string.IsNullOrWhiteSpace(Name) || Name.Length > 256 || Name.Length < 2)
        {
            throw new ArgumentException("Invalid user name.", nameof(Name));
        }

        if (string.IsNullOrWhiteSpace(Phone) || Phone.Length > 15 || Phone.Length < 7)
        {
            throw new ArgumentException("Invalid user phone.", nameof(Phone));
        }

        if (string.IsNullOrWhiteSpace(Role))
        {
            throw new ArgumentException("Invalid role.", nameof(Role));
        }

        // Validar que el rol no sea Owner
        if (Role.Equals("Owner", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Cannot assign Owner role to a user.", nameof(Role));
        }

        // Validar que el rol sea Admin o User
        if (!Role.Equals("Admin", StringComparison.OrdinalIgnoreCase) &&
            !Role.Equals("User", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Role must be Admin or User.", nameof(Role));
        }

        return this;
    }

    public static CreateUserRequest Create(string accountId, string name, string email, string phone, string role)
    {
        return new CreateUserRequest(accountId, name, email, phone, role);
    }
}
