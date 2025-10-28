namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.DTOs;

public class AppInfoDTO
{
    public string Id { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = false;
}