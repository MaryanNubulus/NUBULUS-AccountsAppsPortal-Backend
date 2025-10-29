using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.DTOs;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Enums;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Apps.GetApps;

public interface IGetAppsService
{
    Task<IEnumerable<AppInfoDTO>> GetAppsAsync();

    ResultType ResultType { get; }

    string? Message { get; }
}
