using Microsoft.AspNetCore.Mvc;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Enums;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Apps.PauseResumeApp;

public static class PauseResumeAppEndPoint
{
    public static WebApplication MapPauseResumeAppEndPoint(this WebApplication app)
    {
        app.MapPost("/api/v1/apps/{appId}/pause", async ([FromServices] IPauseResumeAppService pauseResumeAppService,
                                                         [FromRoute] Guid appId) =>
        {
            await pauseResumeAppService.PauseResumeAppAsync(appId, false);
            var result = pauseResumeAppService.ResultType;
            switch (result)
            {
                case ResultType.Ok:
                    return Results.Ok();

                case ResultType.NotFound:
                    return Results.NotFound(new { pauseResumeAppService.Message });

                case ResultType.Error:
                    return Results.Problem(pauseResumeAppService.Message);

                default:
                    return Results.Problem("An unexpected error occurred.");
            }
        }).RequireAuthorization();

        app.MapPost("/api/v1/apps/{appId}/resume", async ([FromServices] IPauseResumeAppService pauseResumeAppService,
                                                          [FromRoute] Guid appId) =>
        {
            await pauseResumeAppService.PauseResumeAppAsync(appId, true);
            var result = pauseResumeAppService.ResultType;
            switch (result)
            {
                case ResultType.Ok:
                    return Results.Ok();

                case ResultType.NotFound:
                    return Results.NotFound(new { pauseResumeAppService.Message });

                case ResultType.Error:
                    return Results.Problem(pauseResumeAppService.Message);

                default:
                    return Results.Problem("An unexpected error occurred.");
            }
        }).RequireAuthorization();

        return app;
    }
}