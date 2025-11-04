using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.DTOs;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Abstractions;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Apps.GetApps;

public interface IGetAppsService
{
    Task<IGenericResponse<IEnumerable<AppInfoDTO>>> ExecuteAsync();
}
