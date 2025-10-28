using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.DTOs;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Responses;

public class GetAppsResponse
{
    public List<AppInfoDTO>? Apps { get; set; } = null;
}