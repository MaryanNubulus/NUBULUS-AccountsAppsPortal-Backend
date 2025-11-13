using Microsoft.AspNetCore.Mvc;
using Nubulus.Backend.Api.Features.Common;

namespace Nubulus.Backend.Api.Features.User.ShareUnshareUser;

public static class ShareUnshareUserEndPoint
{
    public static WebApplication MapShareUnshareUserEndPoint(this WebApplication app)
    {
        app.MapPost(ShareUnshareUserRequest.ShareRoute, async (
            HttpContext context,
            int accountId,
            int userId,
            [FromServices] ShareUnshareUserService service,
            CancellationToken cancellationToken) =>
        {
            var userEmail = context.User.Identities.FirstOrDefault()?.Name ?? "system@nubulus.com";
            var response = await service.ShareAsync(accountId, userId, userEmail, cancellationToken);

            return response.ResultType switch
            {
                ResultType.Ok => Results.Ok(new { message = response.Message }),
                ResultType.NotFound => Results.NotFound(response.Message),
                ResultType.Conflict => Results.Conflict(response.Message),
                _ => Results.Problem(response.Message)
            };
        })
        .WithName("ShareUser")
        .WithTags("Users");

        app.MapDelete(ShareUnshareUserRequest.UnshareRoute, async (
            HttpContext context,
            int accountId,
            int userId,
            [FromServices] ShareUnshareUserService service,
            CancellationToken cancellationToken) =>
        {
            var userEmail = context.User.Identities.FirstOrDefault()?.Name ?? "system@nubulus.com";
            var response = await service.UnshareAsync(accountId, userId, userEmail, cancellationToken);

            return response.ResultType switch
            {
                ResultType.Ok => Results.Ok(new { message = response.Message }),
                ResultType.NotFound => Results.NotFound(response.Message),
                ResultType.Conflict => Results.Conflict(response.Message),
                _ => Results.Problem(response.Message)
            };
        })
        .WithName("UnshareUser")
        .WithTags("Users");

        return app;
    }
}
