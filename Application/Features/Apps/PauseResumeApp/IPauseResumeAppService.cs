namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Apps.PauseResumeApp;

public interface IPauseResumeAppService
{
    Task<bool> PauseResumeAppAsync(Guid Id, bool IsActive);
}
