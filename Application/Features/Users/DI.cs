using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Users.CreateUser;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Users.ExistUser;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Users;

public static class DI
{
    public static IServiceCollection AddUserServices(this IServiceCollection services)
    {
        services.AddTransient<IExistUserService, ExistUserService>();
        services.AddTransient<ICreateUserService, CreateUserService>();
        return services;
    }
}
