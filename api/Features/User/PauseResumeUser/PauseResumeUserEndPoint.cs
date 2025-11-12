using Microsoft.AspNetCore.Mvc;
using Nubulus.Backend.Api.Features.Common;

namespace Nubulus.Backend.Api.Features.User.PauseResumeUser;

public static class PauseResumeUserEndPoint
{
    public static WebApplication MapPauseResumeUserEndPoint(this WebApplication app)
    {
        app.MapPatch(PauseResumeUserRequest.PauseRoute, async (
            HttpContext context,
            int accountId,
            int userId,
            [FromServices] PauseResumeUserService service,
            CancellationToken cancellationToken) =>
        {
            var response = await service.PauseAsync(accountId, userId, context.User.Identities.FirstOrDefault()!.Name!, cancellationToken);
            return response.ResultType switch
            {
                ResultType.Ok => Results.Ok(new { data = response.Data }),
                ResultType.NotFound => Results.NotFound(response.Message),
                _ => Results.Problem(response.Message)
            };
        })
        .WithName("PauseUser")
        .WithTags("Users")
        .RequireAuthorization();

        app.MapPatch(PauseResumeUserRequest.ResumeRoute, async (
            HttpContext context,
            int accountId,
            int userId,
            [FromServices] PauseResumeUserService service,
            CancellationToken cancellationToken) =>
        {
            var response = await service.ResumeAsync(accountId, userId, context.User.Identities.FirstOrDefault()!.Name!, cancellationToken);
            return response.ResultType switch
            {
                ResultType.Ok => Results.Ok(new { data = response.Data }),
                ResultType.NotFound => Results.NotFound(response.Message),
                _ => Results.Problem(response.Message)
            };
        })
        .WithName("ResumeUser")
        .WithTags("Users")
        .RequireAuthorization();

        return app;
    }
}
