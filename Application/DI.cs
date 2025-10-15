using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Abstractions;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Repositories;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Users;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application;

public static class DI
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddTransient<IUsersCommandsRepository, UsersCommandsRepository>();
        services.AddTransient<IUsersQueriesRepository, UsersQueriesRepository>();
        services.AddUserServices();
        return services;
    }
}
