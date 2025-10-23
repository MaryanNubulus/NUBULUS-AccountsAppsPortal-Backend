using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.DTOs;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Apps.CreateApp;

public interface ICreateAppService
{
    Task<bool> CreateAppAsync(CreateAppRequest request);
}
