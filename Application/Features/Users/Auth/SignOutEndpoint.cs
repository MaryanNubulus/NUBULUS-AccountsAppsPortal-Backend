using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Users.Auth;

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

            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var isAuth = context.User?.Identity?.IsAuthenticated ?? false;
            if (!isAuth)
            {
                context.Response.Redirect(returnUrl);
                return;
            }

            var props = new AuthenticationProperties { RedirectUri = returnUrl };
            await context.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme, props);
        }).AllowAnonymous();

        return app;
    }
}