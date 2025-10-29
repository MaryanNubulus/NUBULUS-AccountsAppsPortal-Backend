using Microsoft.AspNetCore.Mvc;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Enums;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Responses;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Apps.GetApps;

public static class GetAppsEndPoint
{
    public static WebApplication MapGetAppsEndPoint(this WebApplication app)
    {
        app.MapGet("/api/v1/apps", async ([FromServices] IGetAppsService getAppsService) =>
        {
            var apps = await getAppsService.GetAppsAsync();
            var result = getAppsService.ResultType;

            switch (result)
            {
                case ResultType.Ok:
                    return Results.Ok(new GetAppsResponse(apps.ToList()));

                case ResultType.Error:
                    return Results.Problem(getAppsService.Message);

                default:
                    return Results.Problem("An unexpected error occurred.");
            }
        }).RequireAuthorization();

        return app;
    }
}