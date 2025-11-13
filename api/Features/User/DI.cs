using Nubulus.Backend.Api.Features.User.CreateUser;
using Nubulus.Backend.Api.Features.User.GetSharedUsers;
using Nubulus.Backend.Api.Features.User.GetUser;
using Nubulus.Backend.Api.Features.User.GetUsers;
using Nubulus.Backend.Api.Features.User.GetUsersToShare;
using Nubulus.Backend.Api.Features.User.PauseResumeUser;
using Nubulus.Backend.Api.Features.User.ShareUnshareUser;
using Nubulus.Backend.Api.Features.User.UpdateUser;

namespace Nubulus.Backend.Api.Features.User;

public static class DI
{
    public static IServiceCollection AddUserFeature(this IServiceCollection services)
    {
        services.AddTransient<CreateUserService>();
        services.AddTransient<GetUsersService>();
        services.AddTransient<GetUserService>();
        services.AddTransient<UpdateUserService>();
        services.AddTransient<PauseResumeUserService>();
        services.AddTransient<GetUsersToShareService>();
        services.AddTransient<ShareUnshareUserService>();
        services.AddTransient<GetSharedUsersService>();
        return services;
    }

    public static WebApplication MapUserEndpoints(this WebApplication app)
    {
        app.MapCreateUserEndPoint();
        app.MapGetUsersEndPoint();
        app.MapGetUserEndPoint();
        app.MapUpdateUserEndPoint();
        app.MapPauseResumeUserEndPoint();
        app.MapGetUsersToShareEndPoint();
        app.MapShareUnshareUserEndPoint();
        app.MapGetSharedUsersEndPoint();
        return app;
    }
}
