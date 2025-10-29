using MongoDB.Driver.Linq;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Enums;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Requests;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Abstractions;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Apps.Common;
namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Apps.CreateApp;

public class CreateAppService : ICreateAppService
{
    private readonly IAppsCommandsRepository _appsCommandsRepository;
    private readonly IAppsQueriesRepository _appsQueriesRepository;
    public CreateAppService(IAppsCommandsRepository appsCommandsRepository, IAppsQueriesRepository appsQueriesRepository)
    {
        _appsCommandsRepository = appsCommandsRepository;
        _appsQueriesRepository = appsQueriesRepository;
    }

    public ResultType ResultType { get; private set; } = ResultType.None;

    public string? Message { get; private set; }

    public Dictionary<string, string[]> ValidationErrors { get; private set; } = new();

    public async Task CreateAppAsync(CreateAppRequest request)
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
            var exists = await _appsQueriesRepository.GetAll().AnyAsync(a => a.Key == request.Key);
            if (exists)
            {
                ResultType = ResultType.Conflict;
                Message = "An app with the same key already exists.";
                return;
            }

            var entity = AppsMappers.CreateEntity(request);

            await _appsCommandsRepository.AddAsync(entity);

            ResultType = ResultType.Ok;
        }
        catch (Exception ex)
        {
            ResultType = ResultType.Error;
            Message = ex.Message;
        }
        await Task.CompletedTask;
    }
}