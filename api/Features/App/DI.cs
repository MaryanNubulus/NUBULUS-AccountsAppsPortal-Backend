using Nubulus.Backend.Api.Features.App.CreateApp;
using Nubulus.Backend.Api.Features.App.GetApp;
using Nubulus.Backend.Api.Features.App.GetApps;
using Nubulus.Backend.Api.Features.App.PauseResumeApp;
using Nubulus.Backend.Api.Features.App.UpdateApp;

namespace Nubulus.Backend.Api.Features.App;

public static class DI
{
    public static IServiceCollection AddAppFeature(this IServiceCollection services)
    {
        services.AddTransient<CreateAppService>();
        services.AddTransient<GetAppsService>();
        services.AddTransient<GetAppService>();
        services.AddTransient<UpdateAppService>();
        services.AddTransient<PauseResumeAppService>();
        return services;
    }

    public static WebApplication MapAppEndpoints(this WebApplication app)
    {
        app.MapCreateAppEndPoint();
        app.MapGetAppsEndPoint();
        app.MapGetAppEndPoint();
        app.MapUpdateAppEndPoint();
        app.MapPauseResumeAppEndPoint();
        return app;
    }
}
