namespace NUBULUS.AccountsAppsPortalBackEnd.Domain.Entities;

public class Account
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = false;
}

