using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Abstractions;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.DTOs;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Mappers;

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

    public async Task<bool> UpdateAppAsync(Guid id, UpdateAppRequest request)
    {
        var entry = AppsMappers.UpdateEntity(await _appsQueriesRepository.GetOneAsync(id), request);
        return await _appsCommandsRepository.UpdateAsync(id, entry);
    }
}