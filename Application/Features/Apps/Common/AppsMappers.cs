using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.DTOs;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Requests;
using NUBULUS.AccountsAppsPortalBackEnd.Domain.Entities;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Apps.Common;

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

    public static App UpdateEntity(App app, UpdateAppRequest request)
    {
        app.Name = request.Name;
        return app;
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