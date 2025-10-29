using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Enums;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Requests;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Apps.CreateApp;

public interface ICreateAppService
{
    Task CreateAppAsync(CreateAppRequest request);

    ResultType ResultType { get; }

    string? Message { get; }

    Dictionary<string, string[]> ValidationErrors { get; }
}
