using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Apps.CreateApp;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Apps.ExistKeyApp;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Apps.GetApps;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Apps.PauseResumeApp;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Apps;

public static class DI
{
    public static IServiceCollection AddAppsServices(this IServiceCollection services)
    {
        services.AddTransient<IExistKeyAppService, ExistKeyAppService>();
        services.AddTransient<ICreateAppService, CreateAppService>();
        services.AddTransient<IGetAppsService, GetAppsService>();
        services.AddTransient<IPauseResumeAppService, PauseResumeAppService>();

        return services;
    }

    public static WebApplication MapAppsEndpoints(this WebApplication app)
    {
        app.MapCreateAppEndPoint();
        app.MapGetAppsEndPoint();
        app.MapPauseResumeAppEndPoint();

        return app;
    }
}