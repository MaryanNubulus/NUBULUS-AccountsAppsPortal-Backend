using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.DTOs;
using NUBULUS.AccountsAppsPortalBackEnd.Domain.Entities;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Mappers;

public static class AppsMappers
{
    public static App CreateEntity(CreateAppRequest request)
    {
        return new App
        {
            Id = Guid.NewGuid(),
            Key = request.Key,
            Name = request.Name,
            IsActive = true,
        };
    }

    public static AppInfoDTO ToDTO(App app)
    {
        return new AppInfoDTO
        {
            Id = app.Id.ToString(),
            Key = app.Key,
            Name = app.Name,
            IsActive = app.IsActive,
        };
    }
}