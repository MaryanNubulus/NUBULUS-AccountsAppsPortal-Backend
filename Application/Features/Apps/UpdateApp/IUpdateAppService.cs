using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Enums;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Requests;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Apps.UpdateApp;

public interface IUpdateAppService
{
    Task UpdateAppAsync(Guid id, UpdateAppRequest request);
    ResultType ResultType { get; }

    string? Message { get; }

    Dictionary<string, string[]> ValidationErrors { get; }
}
