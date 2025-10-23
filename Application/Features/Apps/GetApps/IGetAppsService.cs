using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.DTOs;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Apps.GetApps;

public interface IGetAppsService
{
    Task<IEnumerable<AppInfoDTO>> GetAppsAsync();
}
