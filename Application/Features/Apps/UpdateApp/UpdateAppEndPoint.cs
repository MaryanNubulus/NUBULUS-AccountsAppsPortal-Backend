using Microsoft.AspNetCore.Mvc;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Enums;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Requests;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Apps.UpdateApp;

public static class UpdateAppEndPoint
{
    public static WebApplication MapUpdateAppEndPoint(this WebApplication app)
    {
        app.MapPut("/api/v1/apps/{id:guid}", async ([FromServices] IUpdateAppService updateAppService,
                                                  [FromRoute] Guid id,
                                                  [FromBody] UpdateAppRequest request) =>
        {
            await updateAppService.UpdateAppAsync(id, request);
            var result = updateAppService.ResultType;

            switch (result)
            {
                case ResultType.Ok:
                    return Results.Ok();

                case ResultType.NotFound:
                    return Results.NotFound(new { updateAppService.Message });

                case ResultType.Problems:
                    return Results.ValidationProblem(updateAppService.ValidationErrors);

                case ResultType.Error:
                    return Results.Problem(updateAppService.Message);

                default:
                    return Results.Problem("An unexpected error occurred.");
            }
        }).RequireAuthorization();

        return app;
    }
}