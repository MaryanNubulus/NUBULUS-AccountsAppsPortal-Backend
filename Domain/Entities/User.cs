namespace NUBULUS.AccountsAppsPortalBackEnd.Domain.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.Empty;
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
