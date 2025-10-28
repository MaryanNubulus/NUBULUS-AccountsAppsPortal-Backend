using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.DTOs;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Abstractions;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Mappers;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Apps.GetApps;

public class GetAppsService : IGetAppsService
{
    private readonly IAppsQueriesRepository _appsQueriesRepository;

    public GetAppsService(IAppsQueriesRepository appsQueriesRepository)
    {
        _appsQueriesRepository = appsQueriesRepository;
    }

    public async Task<IEnumerable<AppInfoDTO>> GetAppsAsync()
    {
        return await Task.FromResult(_appsQueriesRepository.GetAll().Select(AppsMappers.ToDTO).ToList());
    }
}