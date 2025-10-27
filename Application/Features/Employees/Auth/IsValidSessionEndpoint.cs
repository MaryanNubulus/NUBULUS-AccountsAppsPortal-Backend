namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Employees.Auth;

public static class IsValidSessionEndpoint
{
    public static WebApplication MapIsValidSessionEndpoint(this WebApplication app)
    {
        app.MapGet("/api/v1/auth/is-valid-session", (HttpContext context) =>
        {
            var isAuthenticated = context.User.Identity?.IsAuthenticated ?? false;
            return isAuthenticated ? Results.Ok() : Results.Unauthorized();
        }).AllowAnonymous();

        return app;
    }
}

