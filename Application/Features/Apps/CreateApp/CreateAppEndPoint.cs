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
            var response = await createAppService.ExecuteAsync(request);

            return response.ResultType switch
            {
                ResultType.Ok => Results.Created($"/api/v1/apps/{response.Data?.Id}", response.Data),
                ResultType.Conflict => Results.Conflict(new { response.Message }),
                ResultType.Problems => Results.ValidationProblem(response.ValidationErrors!),
                _ => Results.Problem(response.Message)
            };
        }).RequireAuthorization();

        return app;
    }
}