using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace Nubulus.Backend.Api.Features.Auth;

public static class SignOutEndpoint
{
    public static WebApplication MapSignOutEndpoint(this WebApplication app)
    {
        app.MapGet("/api/v1/auth/sign-out", async (HttpContext context) =>
        {
            var returnUrl = context.Request.Query["returnUrl"].ToString();
            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                returnUrl = "http://localhost:5173/";
            }

            var props = new AuthenticationProperties { RedirectUri = returnUrl };

            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await context.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme, props);

            return Results.Redirect(returnUrl);
        })
        .WithName("SignOut")
        .WithTags("Auth")
        .AllowAnonymous();

        return app;
    }
}