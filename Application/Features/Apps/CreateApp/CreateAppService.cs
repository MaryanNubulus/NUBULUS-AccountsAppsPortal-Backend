using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Abstractions;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.DTOs;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Mappers;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Apps.CreateApp;

public class CreateAppService : ICreateAppService
{
    private readonly IAppsCommandsRepository _appsCommandsRepository;
    public CreateAppService(IAppsCommandsRepository appsCommandsRepository)
    {
        _appsCommandsRepository = appsCommandsRepository;
    }
    public async Task<bool> CreateAppAsync(CreateAppRequest request)
    {
        var entity = AppsMappers.CreateEntity(request);
        return await _appsCommandsRepository.AddAsync(entity);
    }
}