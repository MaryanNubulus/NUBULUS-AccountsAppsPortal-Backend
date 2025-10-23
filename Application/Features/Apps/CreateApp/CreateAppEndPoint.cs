using Microsoft.AspNetCore.Mvc;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Apps.ExistKeyApp;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.DTOs;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Apps.CreateApp;

public static class CreateAppEndPoint
{
    public static WebApplication MapCreateAppEndPoint(this WebApplication app)
    {
        app.MapPost("/api/v1/apps", async ([FromServices] ICreateAppService createAppService,
                                           [FromServices] IExistKeyAppService existKeyAppService,
                                           [FromBody] CreateAppRequest request) =>
        {
            var keyExists = await existKeyAppService.ExistKeyAppAsync(request.Key);
            if (keyExists) return Results.Conflict();

            return await createAppService.CreateAppAsync(request)
                ? Results.Ok()
                : Results.BadRequest();
        }).RequireAuthorization();

        return app;
    }
}