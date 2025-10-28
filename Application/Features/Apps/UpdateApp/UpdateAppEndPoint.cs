using Microsoft.AspNetCore.Mvc;
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
            return await updateAppService.UpdateAppAsync(id, request)
                ? Results.Ok()
                : Results.BadRequest();
        }).RequireAuthorization();

        return app;
    }
}