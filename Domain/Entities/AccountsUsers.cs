namespace NUBULUS.AccountsAppsPortalBackEnd.Domain.Entities;

public class AccountsUsers
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid AccountId { get; set; } = Guid.Empty;

    public Guid UserId { get; set; } = Guid.Empty;

    public AccountsUsersRole Role { get; set; } = AccountsUsersRole.User;
}

public enum AccountsUsersRole
{
    Owner,
    Admin,
    User
}