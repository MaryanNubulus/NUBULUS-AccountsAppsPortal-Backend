using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Users.CreateUser;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Users.GetUsers;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Users.UpdateUser;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Users.ActivateDeactivateUser;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Users;

public static class DI
{
    public static IServiceCollection AddUsersServices(this IServiceCollection services)
    {
        services.AddTransient<ICreateUserService, CreateUserService>();
        services.AddTransient<IGetUsersService, GetUsersService>();
        services.AddTransient<IUpdateUserService, UpdateUserService>();
        services.AddTransient<IActivateDeactivateUserService, ActivateDeactivateUserService>();

        return services;
    }

    public static WebApplication MapUsersEndpoints(this WebApplication app)
    {
        app.MapCreateUserEndpoint();
        app.MapGetUsersEndpoint();
        app.MapUpdateUserEndpoint();
        app.MapActivateDeactivateUserEndpoint();

        return app;
    }
}
