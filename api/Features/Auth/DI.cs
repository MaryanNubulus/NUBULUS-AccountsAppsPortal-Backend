namespace Nubulus.Backend.Api.Features.Auth;

public static class DI
{
    public static WebApplication MapAuthEndpoints(this WebApplication app)
    {
        app.MapSignInEndpoint()
           .MapSignOutEndpoint()
           .MapIsValidSessionEndpoint();

        return app;
    }
}