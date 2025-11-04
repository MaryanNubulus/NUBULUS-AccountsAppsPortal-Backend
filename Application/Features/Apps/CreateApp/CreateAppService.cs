using MongoDB.Driver.Linq;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.DTOs;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Enums;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Requests;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Abstractions;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Apps.Common;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Apps.CreateApp;

internal sealed class CreateAppResponse : IGenericResponse<AppInfoDTO>
{
    public ResultType ResultType { get; init; }
    public string? Message { get; init; }
    public Dictionary<string, string[]>? ValidationErrors { get; init; }
    public AppInfoDTO? Data { get; init; }

    private CreateAppResponse(ResultType resultType, string? message, Dictionary<string, string[]>? validationErrors, AppInfoDTO? data)
    {
        ResultType = resultType;
        Message = message;
        ValidationErrors = validationErrors;
        Data = data;
    }

    public static CreateAppResponse Success() =>
        new(ResultType.Ok, "App created successfully", null, null);

    public static CreateAppResponse Conflict(string message) =>
        new(ResultType.Conflict, message, null, null);

    public static CreateAppResponse Error(string message) =>
        new(ResultType.Error, message, null, null);

    public static CreateAppResponse ValidationError(Dictionary<string, string[]> errors) =>
        new(ResultType.Problems, "Validation errors", errors, null);
}

public class CreateAppService : ICreateAppService
{
    private readonly IAppsCommandsRepository _appsCommandsRepository;
    private readonly IAppsQueriesRepository _appsQueriesRepository;

    public CreateAppService(IAppsCommandsRepository appsCommandsRepository, IAppsQueriesRepository appsQueriesRepository)
    {
        _appsCommandsRepository = appsCommandsRepository;
        _appsQueriesRepository = appsQueriesRepository;
    }

    public async Task<IGenericResponse<AppInfoDTO>> ExecuteAsync(CreateAppRequest request)
    {
        try
        {
            request.Validate();
        }
        catch (Exception ex)
        {
            return CreateAppResponse.ValidationError(new Dictionary<string, string[]>
            {
                { "request", new[] { ex.Message } }
            });
        }

        var exists = await _appsQueriesRepository.GetAll().AnyAsync(a => a.Key == request.Key);
        if (exists)
        {
            return CreateAppResponse.Conflict("An app with the same key already exists.");
        }

        try
        {
            var entity = AppsMappers.CreateEntity(request);
            await _appsCommandsRepository.AddAsync(entity);

            return CreateAppResponse.Success();
        }
        catch (Exception ex)
        {
            return CreateAppResponse.Error($"An error occurred while creating the app: {ex.Message}");
        }
    }
}