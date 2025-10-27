using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.DTOs;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Apps.UpdateApp;

public interface IUpdateAppService
{
    Task<bool> UpdateAppAsync(Guid id, UpdateAppRequest request);
}
