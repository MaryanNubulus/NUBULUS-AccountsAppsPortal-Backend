namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Users.Auth;
public static class IsValidSessionEndpoint
{
    public static WebApplication MapIsValidSessionEndpoint(this WebApplication app)
    {
        app.MapGet("/api/v1/auth/session-is-valid", (HttpContext context) =>
        {
            var isAuthenticated = context.User.Identity?.IsAuthenticated ?? false;
            return isAuthenticated ? Results.Ok() : Results.Unauthorized();
        }).AllowAnonymous();

        return app;
    }
}

                