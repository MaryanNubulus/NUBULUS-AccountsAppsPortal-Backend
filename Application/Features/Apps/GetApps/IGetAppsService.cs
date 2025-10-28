using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.DTOs;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Apps.GetApps;

public interface IGetAppsService
{
    Task<IEnumerable<AppInfoDTO>> GetAppsAsync();
}
