using Microsoft.AspNetCore.Mvc;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Enums;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.ValueObjects;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Apps.PauseResumeApp;

public static class PauseResumeAppEndPoint
{
    public static WebApplication MapPauseResumeAppEndPoint(this WebApplication app)
    {
        app.MapPost("/api/v1/apps/{appId}/pause", async ([FromServices] IPauseResumeAppService pauseResumeAppService,
                                                         [FromRoute] Guid appId) =>
        {
            var appIdVO = IdObject.Create(appId);
            var response = await pauseResumeAppService.ExecuteAsync(appIdVO, false);

            return response.ResultType switch
            {
                ResultType.Ok => Results.Ok(response.Data),
                ResultType.NotFound => Results.NotFound(new { response.Message }),
                ResultType.Problems => Results.ValidationProblem(new Dictionary<string, string[]> { { "id", new[] { response.Message ?? "Invalid ID" } } }),
                _ => Results.Problem(response.Message)
            };
        }).RequireAuthorization();

        app.MapPost("/api/v1/apps/{appId}/resume", async ([FromServices] IPauseResumeAppService pauseResumeAppService,
                                                          [FromRoute] Guid appId) =>
        {
            var appIdVO = IdObject.Create(appId);
            var response = await pauseResumeAppService.ExecuteAsync(appIdVO, true);

            return response.ResultType switch
            {
                ResultType.Ok => Results.Ok(response.Data),
                ResultType.NotFound => Results.NotFound(new { response.Message }),
                ResultType.Problems => Results.ValidationProblem(new Dictionary<string, string[]> { { "id", new[] { response.Message ?? "Invalid ID" } } }),
                _ => Results.Problem(response.Message)
            };
        }).RequireAuthorization();

        return app;
    }
}