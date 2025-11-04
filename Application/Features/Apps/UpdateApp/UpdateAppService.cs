using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.DTOs;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Enums;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Requests;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.ValueObjects;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Abstractions;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Apps.Common;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Apps.UpdateApp;

internal sealed class UpdateAppResponse : IGenericResponse<AppInfoDTO>
{
    public ResultType ResultType { get; init; }
    public string? Message { get; init; }
    public Dictionary<string, string[]>? ValidationErrors { get; init; }
    public AppInfoDTO? Data { get; init; }

    public static IGenericResponse<AppInfoDTO> Success(AppInfoDTO data) => new UpdateAppResponse
    {
        ResultType = ResultType.Ok,
        Data = data
    };

    public static IGenericResponse<AppInfoDTO> NotFound(string message) => new UpdateAppResponse
    {
        ResultType = ResultType.NotFound,
        Message = message
    };

    public static IGenericResponse<AppInfoDTO> Error(string message) => new UpdateAppResponse
    {
        ResultType = ResultType.Error,
        Message = message
    };

    public static IGenericResponse<AppInfoDTO> ValidationError(string field, string message) => new UpdateAppResponse
    {
        ResultType = ResultType.Problems,
        ValidationErrors = new Dictionary<string, string[]> { { field, new[] { message } } }
    };
}

public class UpdateAppService : IUpdateAppService
{
    private readonly IAppsCommandsRepository _appsCommandsRepository;
    private readonly IAppsQueriesRepository _appsQueriesRepository;

    public UpdateAppService(IAppsCommandsRepository appsCommandsRepository, IAppsQueriesRepository appsQueriesRepository)
    {
        _appsCommandsRepository = appsCommandsRepository;
        _appsQueriesRepository = appsQueriesRepository;
    }

    public async Task<IGenericResponse<AppInfoDTO>> ExecuteAsync(IdObject id, UpdateAppRequest request)
    {
        try
        {
            IdObject.ValidateId(id.Value);
            request.Validate();
        }
        catch (Exception ex)
        {
            return UpdateAppResponse.ValidationError("Request", ex.Message);
        }

        try
        {
            var app = await _appsQueriesRepository.GetOneAsync(id.Value);
            if (app == null)
            {
                return UpdateAppResponse.NotFound("App not found.");
            }

            AppsMappers.UpdateEntity(app, request);

            await _appsCommandsRepository.UpdateAsync(id.Value, app);

            var appDto = AppsMappers.ToDTO(app);
            return UpdateAppResponse.Success(appDto);
        }
        catch (Exception ex)
        {
            return UpdateAppResponse.Error(ex.Message);
        }
    }
}