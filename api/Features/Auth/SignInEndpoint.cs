namespace Nubulus.Backend.Api.Features.Auth;

public static class SignInEndpoint
{
    public static WebApplication MapSignInEndpoint(this WebApplication app)
    {
        app.MapGet("/api/v1/auth/sign-in", () =>
        {
            return Results.Redirect("/api/v1/auth/success");
        })
        .WithName("SignIn")
        .WithTags("Auth")
        .RequireAuthorization();

        app.MapGet("/api/v1/auth/success", (HttpContext context) =>
        {
            var employeeEmail = context.User.Identities.FirstOrDefault()!.Name!;
            var employeeName = context.User.Claims.FirstOrDefault(c => c.Type == "name")?.Value ?? "Unknown";

            return Results.Redirect("http://localhost:5173/private");

        })
        .WithName("SignInSuccess")
        .WithTags("Auth")
        .RequireAuthorization();

        return app;
    }
}
