using Nubulus.Backend.Api.Application.Features.Auth;
using Nubulus.Backend.Api.Features.Account;

namespace Nubulus.Backend.Api.Features;

public static class DI
{
    public static IServiceCollection AddApplicationFeature(this IServiceCollection services)
    {
        services.AddAccountFeature();
        return services;
    }

    public static WebApplication MapApplicationEndpoints(this WebApplication app)
    {
        app.MapAuthEndpoints();
        app.MapAccountEndpoints();
        return app;
    }
}