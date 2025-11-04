using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.ValueObjects;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Requests;

public record UpdateAccountRequest
{
    public string Name { get; init; }
    public string UserName { get; init; }
    public string UserEmail { get; init; }
    public string UserPhone { get; init; }

    public UpdateAccountRequest()
    {
        Name = string.Empty;
        UserName = string.Empty;
        UserEmail = string.Empty;
        UserPhone = string.Empty;
    }

    private UpdateAccountRequest(string name, string userName, string userEmail, string userPhone)
    {
        Name = name;
        UserName = userName;
        UserPhone = userPhone;
        UserEmail = Email.ValidateEmail(userEmail);
        Validate();
    }
    public UpdateAccountRequest Validate()
    {
        if (string.IsNullOrWhiteSpace(Name) || Name.Length > 256 || Name.Length < 2)
        {
            throw new ArgumentException("Invalid account name.", nameof(Name));
        }
        if (string.IsNullOrWhiteSpace(UserName) || UserName.Length > 256 || UserName.Length < 2)
        {
            throw new ArgumentException("Invalid user name.", nameof(UserName));
        }
        if (string.IsNullOrWhiteSpace(UserPhone) || UserPhone.Length > 15 || UserPhone.Length < 7)
        {
            throw new ArgumentException("Invalid user phone.", nameof(UserPhone));
        }

        return this;
    }
    public static UpdateAccountRequest Create(string name, string userName, string userEmail, string userPhone)
    {
        return new UpdateAccountRequest(name, userName, userEmail, userPhone);
    }
}