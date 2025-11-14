using Nubulus.Backend.Api.Features.Auth;
using Nubulus.Backend.Api.Features.Account;
using Nubulus.Backend.Api.Features.App;
using Nubulus.Backend.Api.Features.Employees;
using Nubulus.Backend.Api.Features.User;

namespace Nubulus.Backend.Api.Features;

public static class DI
{
    public static IServiceCollection AddApplicationFeature(this IServiceCollection services)
    {
        services.AddAccountFeature();
        services.AddUserFeature();
        services.AddAppFeature();
        return services;
    }

    public static WebApplication MapApplicationEndpoints(this WebApplication app)
    {
        app.MapAuthEndpoints();
        app.MapAccountEndpoints();
        app.MapUserEndpoints();
        app.MapEmployeeEndpoints();
        app.MapAppEndpoints();
        return app;
    }
}