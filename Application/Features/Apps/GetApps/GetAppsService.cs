using MongoDB.Driver.Linq;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.DTOs;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Enums;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Abstractions;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Apps.Common;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Apps.GetApps;

internal sealed class GetAppsResponse : IGenericResponse<IEnumerable<AppInfoDTO>>
{
    public ResultType ResultType { get; init; }
    public string? Message { get; init; }
    public Dictionary<string, string[]>? ValidationErrors { get; init; }
    public IEnumerable<AppInfoDTO>? Data { get; init; }

    public static IGenericResponse<IEnumerable<AppInfoDTO>> Success(IEnumerable<AppInfoDTO> data) => new GetAppsResponse
    {
        ResultType = ResultType.Ok,
        Data = data
    };

    public static IGenericResponse<IEnumerable<AppInfoDTO>> Error(string message) => new GetAppsResponse
    {
        ResultType = ResultType.Error,
        Message = message,
        Data = Enumerable.Empty<AppInfoDTO>()
    };
}

public class GetAppsService : IGetAppsService
{
    private readonly IAppsQueriesRepository _appsQueriesRepository;

    public GetAppsService(IAppsQueriesRepository appsQueriesRepository)
    {
        _appsQueriesRepository = appsQueriesRepository;
    }

    public async Task<IGenericResponse<IEnumerable<AppInfoDTO>>> ExecuteAsync()
    {
        try
        {
            var appsEntities = await _appsQueriesRepository.GetAll().ToListAsync();
            if (appsEntities == null || !appsEntities.Any())
            {
                return GetAppsResponse.Success(Enumerable.Empty<AppInfoDTO>());
            }

            var appsList = appsEntities.Select(AppsMappers.ToDTO).ToList();
            return GetAppsResponse.Success(appsList);
        }
        catch (Exception ex)
        {
            return GetAppsResponse.Error(ex.Message);
        }
    }
}