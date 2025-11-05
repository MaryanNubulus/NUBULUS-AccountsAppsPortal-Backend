using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.ValueObjects;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Requests;

public record UpdateUserRequest
{
    public string Name { get; init; }
    public string Email { get; init; }
    public string Phone { get; init; }
    public string Role { get; init; }

    public UpdateUserRequest()
    {
        Name = string.Empty;
        Email = string.Empty;
        Phone = string.Empty;
        Role = string.Empty;
    }

    private UpdateUserRequest(string name, string email, string phone, string role)
    {
        Name = name;
        Email = NUBULUS.AccountsAppsPortalBackEnd.Application.Common.ValueObjects.Email.ValidateEmail(email);
        Phone = phone;
        Role = role;
        Validate();
    }

    public UpdateUserRequest Validate()
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

        return this;
    }

    public static UpdateUserRequest Create(string name, string email, string phone, string role)
    {
        return new UpdateUserRequest(name, email, phone, role);
    }
}
