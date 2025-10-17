using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Abstractions;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Repositories;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Users;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Users.Auth;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Users.GetUsers;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application;

public static class DI
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddTransient<IUsersCommandsRepository, UsersCommandsRepository>();
        services.AddTransient<IUsersQueriesRepository, UsersQueriesRepository>();
        services.AddUserServices();
        return services;
    }

    public static WebApplication MapApplicationEndpoints(this WebApplication app)
    {

        app.MapSignInEndpoint().MapSignOutEndpoint().MapIsValidSessionEndpoint();

        app.MapGetUsersEndPoint();

        return app;
    }
}
