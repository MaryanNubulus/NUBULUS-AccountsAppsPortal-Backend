using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Enums;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Abstractions;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Apps.PauseResumeApp;

public class PauseResumeAppService : IPauseResumeAppService
{
    private readonly IAppsQueriesRepository _appsQueriesRepository;
    private readonly IAppsCommandsRepository _appsCommandsRepository;

    public ResultType ResultType { get; private set; } = ResultType.None;

    public string? Message { get; private set; }


    public PauseResumeAppService(IAppsQueriesRepository appsQueriesRepository, IAppsCommandsRepository appsCommandsRepository)
    {
        _appsQueriesRepository = appsQueriesRepository;
        _appsCommandsRepository = appsCommandsRepository;
    }

    public async Task PauseResumeAppAsync(Guid id, bool isActive)
    {
        try
        {
            var app = await _appsQueriesRepository.GetOneAsync(id);
            if (app == null)
            {
                ResultType = ResultType.NotFound;
                Message = "App not found.";
                return;
            }

            app.IsActive = isActive;

            await _appsCommandsRepository.UpdateAsync(id, app);

            ResultType = ResultType.Ok;
        }
        catch (Exception ex)
        {
            ResultType = ResultType.Error;
            Message = ex.Message;
        }
    }
}