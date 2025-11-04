using Microsoft.AspNetCore.Mvc;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Enums;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Requests;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.ValueObjects;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Apps.UpdateApp;

public static class UpdateAppEndPoint
{
    public static WebApplication MapUpdateAppEndPoint(this WebApplication app)
    {
        app.MapPut("/api/v1/apps/{id:guid}", async ([FromServices] IUpdateAppService updateAppService,
                                                  [FromRoute] Guid id,
                                                  [FromBody] UpdateAppRequest request) =>
        {
            var idVO = IdObject.Create(id);
            var response = await updateAppService.ExecuteAsync(idVO, request);

            return response.ResultType switch
            {
                ResultType.Ok => Results.Ok(response.Data),
                ResultType.NotFound => Results.NotFound(new { response.Message }),
                ResultType.Problems => Results.ValidationProblem(response.ValidationErrors!),
                _ => Results.Problem(response.Message)
            };
        }).RequireAuthorization();

        return app;
    }
}