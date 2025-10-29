using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Enums;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Requests;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Abstractions;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Apps.Common;
namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Apps.UpdateApp;

public class UpdateAppService : IUpdateAppService
{
    private readonly IAppsCommandsRepository _appsCommandsRepository;
    private readonly IAppsQueriesRepository _appsQueriesRepository;

    public UpdateAppService(IAppsCommandsRepository appsCommandsRepository, IAppsQueriesRepository appsQueriesRepository)
    {
        _appsCommandsRepository = appsCommandsRepository;
        _appsQueriesRepository = appsQueriesRepository;
    }

    public ResultType ResultType { get; private set; } = ResultType.None;

    public string? Message { get; private set; }

    public Dictionary<string, string[]> ValidationErrors { get; private set; } = new();

    public async Task UpdateAppAsync(Guid id, UpdateAppRequest request)
    {
        try
        {
            request.Validate();
        }
        catch (Exception ex)
        {
            ResultType = ResultType.Problems;
            ValidationErrors = new() { { "Request", new[] { ex.Message } } };
            return;
        }
        try
        {
            var app = await _appsQueriesRepository.GetOneAsync(id);
            if (app == null)
            {
                ResultType = ResultType.NotFound;
                Message = "App not found.";
                return;
            }

            AppsMappers.UpdateEntity(app, request);

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