using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Apps.CreateApp;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Apps.ExistKeyApp;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Apps.GetApps;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Apps;

public static class DI
{
    public static IServiceCollection AddAppsServices(this IServiceCollection services)
    {
        services.AddTransient<IExistKeyAppService, ExistKeyAppService>();
        services.AddTransient<ICreateAppService, CreateAppService>();
        services.AddTransient<IGetAppsService, GetAppsService>();

        return services;
    }

    public static WebApplication MapAppsEndpoints(this WebApplication app)
    {
        app.MapCreateAppEndPoint();
        app.MapGetAppsEndPoint();

        return app;
    }
}