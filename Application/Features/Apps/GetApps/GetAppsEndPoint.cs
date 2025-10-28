using Microsoft.AspNetCore.Mvc;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Responses;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Apps.GetApps;

public static class GetAppsEndPoint
{
    public static WebApplication MapGetAppsEndPoint(this WebApplication app)
    {
        app.MapGet("/api/v1/apps", async ([FromServices] IGetAppsService getAppsService) =>
        {
            var apps = await getAppsService.GetAppsAsync();
            var response = new GetAppsResponse();

            if (apps == null || !apps.Any())
            {
                return Results.Ok(response);
            }
            response.Apps = apps.ToList();

            return Results.Ok(response);
        }).RequireAuthorization();

        return app;
    }
}