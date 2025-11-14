using Microsoft.AspNetCore.Mvc;
using Nubulus.Backend.Api.Features.Common;

namespace Nubulus.Backend.Api.Features.App.PauseResumeApp;

public static class PauseResumeAppEndPoint
{
    public static WebApplication MapPauseResumeAppEndPoint(this WebApplication app)
    {
        // Endpoint para pausar app
        app.MapPatch("/api/v1/apps/{id}/pause", async (
            HttpContext context,
            [FromRoute] int id,
            [FromServices] PauseResumeAppService service,
            CancellationToken cancellationToken) =>
        {
            var response = await service.PauseAsync(id, context.User.Identities.FirstOrDefault()!.Name!, cancellationToken);

            return response.ResultType switch
            {
                ResultType.Ok => Results.NoContent(),
                ResultType.NotFound => Results.NotFound(response.Message),
                _ => Results.Problem(response.Message)
            };
        })
        .WithName("PauseApp")
        .WithTags("Apps")
        .RequireAuthorization();

        // Endpoint para reactivar app
        app.MapPatch("/api/v1/apps/{id}/resume", async (
            HttpContext context,
            [FromRoute] int id,
            [FromServices] PauseResumeAppService service,
            CancellationToken cancellationToken) =>
        {
            var response = await service.ResumeAsync(id, context.User.Identities.FirstOrDefault()!.Name!, cancellationToken);

            return response.ResultType switch
            {
                ResultType.Ok => Results.NoContent(),
                ResultType.NotFound => Results.NotFound(response.Message),
                _ => Results.Problem(response.Message)
            };
        })
        .WithName("ResumeApp")
        .WithTags("Apps")
        .RequireAuthorization();

        return app;
    }
}
