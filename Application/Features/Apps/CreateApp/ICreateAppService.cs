using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Requests;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Apps.CreateApp;

public interface ICreateAppService
{
    Task<bool> CreateAppAsync(CreateAppRequest request);
}
