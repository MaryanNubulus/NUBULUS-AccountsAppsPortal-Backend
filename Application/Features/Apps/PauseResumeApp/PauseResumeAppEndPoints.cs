using Microsoft.AspNetCore.Mvc;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Apps.PauseResumeApp;

public static class PauseResumeAppEndPoint
{
    public static WebApplication MapPauseResumeAppEndPoint(this WebApplication app)
    {
        app.MapPost("/api/v1/apps/{appId}/pause", async ([FromServices] IPauseResumeAppService pauseResumeAppService,
                                                         [FromRoute] Guid appId) =>
        {
            return await pauseResumeAppService.PauseResumeAppAsync(appId, false)
                ? Results.Ok()
                : Results.BadRequest();
        }).RequireAuthorization();

        app.MapPost("/api/v1/apps/{appId}/resume", async ([FromServices] IPauseResumeAppService pauseResumeAppService,
                                                          [FromRoute] Guid appId) =>
        {
            return await pauseResumeAppService.PauseResumeAppAsync(appId, true)
                ? Results.Ok()
                : Results.BadRequest();
        }).RequireAuthorization();

        return app;
    }
}