using Microsoft.AspNetCore.Mvc;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.DTOs;

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
                response.Success = false;
                response.Message = "No apps found.";
                return Results.Ok(response);
            }
            response.Success = true;
            response.Apps = apps.ToList();

            return Results.Ok(response);
        }).RequireAuthorization();

        return app;
    }
}