using MongoDB.Driver.Linq;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.DTOs;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Enums;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Abstractions;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Apps.Common;
namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Apps.GetApps;

public class GetAppsService : IGetAppsService
{
    private readonly IAppsQueriesRepository _appsQueriesRepository;

    public GetAppsService(IAppsQueriesRepository appsQueriesRepository)
    {
        _appsQueriesRepository = appsQueriesRepository;
    }

    public ResultType ResultType { get; private set; } = ResultType.None;

    public string? Message { get; private set; }

    public async Task<IEnumerable<AppInfoDTO>> GetAppsAsync()
    {
        var appsList = new List<AppInfoDTO>();
        try
        {
            var appsEntities = await _appsQueriesRepository.GetAll().ToListAsync();
            if (appsEntities == null || !appsEntities.Any())
            {
                ResultType = ResultType.Ok;
                Message = "No apps found.";
                return appsList;
            }

            appsList = appsEntities.Select(AppsMappers.ToDTO).ToList();
            ResultType = ResultType.Ok;
        }
        catch (Exception ex)
        {
            ResultType = ResultType.Error;
            Message = ex.Message;
            return appsList;
        }

        return appsList;
    }
}