using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Requests;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Apps.UpdateApp;

public interface IUpdateAppService
{
    Task<bool> UpdateAppAsync(Guid id, UpdateAppRequest request);
}
