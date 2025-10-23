using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Abstractions;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Apps;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Repositories;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Users;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application;

public static class DI
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddTransient<IUsersCommandsRepository, UsersCommandsRepository>();
        services.AddTransient<IUsersQueriesRepository, UsersQueriesRepository>();
        services.AddUserServices();

        services.AddTransient<IAppsCommandsRepository, AppsCommandsRepository>();
        services.AddTransient<IAppsQueriesRepository, AppsQueriesRepository>();
        services.AddAppsServices();

        return services;
    }

    public static WebApplication MapApplicationEndpoints(this WebApplication app)
    {
        app.MapAppsEndpoints();
        app.MapUsersEndpoints();

        return app;
    }
}
