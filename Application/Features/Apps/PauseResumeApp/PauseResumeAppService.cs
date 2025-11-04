using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.DTOs;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Enums;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.ValueObjects;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Abstractions;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Apps.Common;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Apps.PauseResumeApp;

internal sealed class PauseResumeAppResponse : IGenericResponse<AppInfoDTO>
{
    public ResultType ResultType { get; init; }
    public string? Message { get; init; }
    public Dictionary<string, string[]>? ValidationErrors { get; init; }
    public AppInfoDTO? Data { get; init; }

    public static IGenericResponse<AppInfoDTO> Success(AppInfoDTO data) => new PauseResumeAppResponse
    {
        ResultType = ResultType.Ok,
        Data = data
    };

    public static IGenericResponse<AppInfoDTO> NotFound(string message) => new PauseResumeAppResponse
    {
        ResultType = ResultType.NotFound,
        Message = message
    };

    public static IGenericResponse<AppInfoDTO> Error(string message) => new PauseResumeAppResponse
    {
        ResultType = ResultType.Error,
        Message = message
    };

    public static IGenericResponse<AppInfoDTO> ValidationError(string message) => new PauseResumeAppResponse
    {
        ResultType = ResultType.Problems,
        Message = message
    };
}

public class PauseResumeAppService : IPauseResumeAppService
{
    private readonly IAppsQueriesRepository _appsQueriesRepository;
    private readonly IAppsCommandsRepository _appsCommandsRepository;

    public PauseResumeAppService(IAppsQueriesRepository appsQueriesRepository, IAppsCommandsRepository appsCommandsRepository)
    {
        _appsQueriesRepository = appsQueriesRepository;
        _appsCommandsRepository = appsCommandsRepository;
    }

    public async Task<IGenericResponse<AppInfoDTO>> ExecuteAsync(IdObject id, bool isActive)
    {
        try
        {
            IdObject.ValidateId(id.Value);
        }
        catch (ArgumentException ex)
        {
            return PauseResumeAppResponse.ValidationError(ex.Message);
        }

        try
        {
            var app = await _appsQueriesRepository.GetOneAsync(id.Value);
            if (app == null)
            {
                return PauseResumeAppResponse.NotFound("App not found.");
            }

            app.IsActive = isActive;

            await _appsCommandsRepository.UpdateAsync(id.Value, app);

            var appDto = AppsMappers.ToDTO(app);
            return PauseResumeAppResponse.Success(appDto);
        }
        catch (Exception ex)
        {
            return PauseResumeAppResponse.Error(ex.Message);
        }
    }
}