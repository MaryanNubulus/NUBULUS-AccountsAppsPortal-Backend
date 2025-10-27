using Microsoft.AspNetCore.Mvc;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Users.GetCurrentUser;

public static class GetCurrentUserEndPoint
{
    public static WebApplication MapGetCurrentUserEndPoint(this WebApplication app)
    {
        app.MapGet("/api/v1/users/current", async (HttpContext context,
            [FromServices] IGetCurrentUserService getCurrentUserService) =>
        {
            var email = context.User.Identities.FirstOrDefault()!.Name!;
            var userInfo = await getCurrentUserService.GetCurrentUserAsync(email);
            return userInfo != null
                ? Results.Ok(userInfo)
                : Results.NotFound();
        }).RequireAuthorization();

        return app;
    }
}