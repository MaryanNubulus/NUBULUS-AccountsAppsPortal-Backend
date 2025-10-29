using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Enums;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Apps.PauseResumeApp;

public interface IPauseResumeAppService
{
    Task PauseResumeAppAsync(Guid id, bool isActive);

    ResultType ResultType { get; }

    string? Message { get; }
}
