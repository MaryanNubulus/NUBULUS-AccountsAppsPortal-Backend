using Microsoft.AspNetCore.Mvc;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Enums;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Apps.GetApps;

public static class GetAppsEndPoint
{
    public static WebApplication MapGetAppsEndPoint(this WebApplication app)
    {
        app.MapGet("/api/v1/apps", async ([FromServices] IGetAppsService getAppsService) =>
        {
            var response = await getAppsService.ExecuteAsync();

            return response.ResultType switch
            {
                ResultType.Ok => Results.Ok(response.Data),
                _ => Results.Problem(response.Message)
            };
        }).RequireAuthorization();

        return app;
    }
}