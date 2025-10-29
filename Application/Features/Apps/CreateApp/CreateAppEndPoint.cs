using Microsoft.AspNetCore.Mvc;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Enums;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Requests;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Apps.CreateApp;

public static class CreateAppEndPoint
{
    public static WebApplication MapCreateAppEndPoint(this WebApplication app)
    {
        app.MapPost("/api/v1/apps", async ([FromServices] ICreateAppService createAppService,
                                           [FromBody] CreateAppRequest request) =>
        {
            await createAppService.CreateAppAsync(request);
            var result = createAppService.ResultType;

            switch (result)
            {
                case ResultType.Ok:
                    return Results.Created();

                case ResultType.Conflict:
                    return Results.Conflict(new { createAppService.Message });

                case ResultType.Problems:
                    return Results.ValidationProblem(createAppService.ValidationErrors);

                case ResultType.Error:
                    return Results.Problem(createAppService.Message);

                default:
                    return Results.Problem("An unexpected error occurred.");
            }
        }).RequireAuthorization();

        return app;
    }
}