namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.DTOs;

public class GetAppsResponse
{
    public bool Success { get; set; } = false;
    public string? Message { get; set; } = null;
    public List<AppInfoDTO>? Apps { get; set; } = null;
}