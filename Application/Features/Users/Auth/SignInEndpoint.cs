using Microsoft.AspNetCore.Mvc;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.DTOs;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Users.CreateUser;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Users.ExistUser;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Users.Auth;

public static class SignInEndpoint
{
    public static WebApplication MapSignInEndpoint(this WebApplication app)
    {
        app.MapGet("/api/v1/auth/sign-in", () =>
        {
            return Results.Redirect("/api/v1/auth/success");
        }).RequireAuthorization();

        app.MapGet("/api/v1/auth/success", async (HttpContext context, [FromServices] IExistUserService existUserService, [FromServices] ICreateUserService createUserService) =>
        {
            var userEmail = context.User.Identities.FirstOrDefault()!.Name!;
            var userExists = await existUserService.ExistUserAsync(userEmail);

            if (!userExists)
            {
                var userName = context.User.Claims.FirstOrDefault(c => c.Type == "name")?.Value ?? "Unknown";

                var request = new CreateUserRequest
                {
                    Email = userEmail,
                    Name = userName
                };
                var created = await createUserService.CreateUserAsync(request);
                if (!created)
                {
                    return Results.StatusCode(500);
                }
            }
            return Results.Redirect("http://localhost:5173/private");
        }).RequireAuthorization();

        return app;
    }
}

