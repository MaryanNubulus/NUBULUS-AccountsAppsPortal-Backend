namespace NUBULUS.AccountsAppsPortalBackEnd.Domain.Entities;

public class App
{
    public Guid Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = false;
}