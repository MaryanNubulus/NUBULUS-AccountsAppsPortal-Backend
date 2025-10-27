using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Abstractions;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Apps.PauseResumeApp;

public class PauseResumeAppService : IPauseResumeAppService
{
    private readonly IAppsQueriesRepository _appsQueriesRepository;
    private readonly IAppsCommandsRepository _appsCommandsRepository;

    public PauseResumeAppService(IAppsQueriesRepository appsQueriesRepository, IAppsCommandsRepository appsCommandsRepository)
    {
        _appsQueriesRepository = appsQueriesRepository;
        _appsCommandsRepository = appsCommandsRepository;
    }

    public async Task<bool> PauseResumeAppAsync(Guid Id, bool IsActive)
    {
        var app = await _appsQueriesRepository.GetOneAsync(Id);
        app.IsActive = IsActive;
        return await _appsCommandsRepository.UpdateAsync(Id, app);
    }
}