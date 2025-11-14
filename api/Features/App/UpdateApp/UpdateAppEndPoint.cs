using Microsoft.AspNetCore.Mvc;
using Nubulus.Backend.Api.Features.Common;

namespace Nubulus.Backend.Api.Features.App.UpdateApp;

public static class UpdateAppEndPoint
{
    public static WebApplication MapUpdateAppEndPoint(this WebApplication app)
    {
        app.MapPut(UpdateAppRequest.Route, async (
            HttpContext context,
            [FromRoute] int id,
            [FromBody] UpdateAppRequest request,
            [FromServices] UpdateAppService service,
            CancellationToken cancellationToken) =>
        {
            var response = await service.ExecuteAsync(id, request, context.User.Identities.FirstOrDefault()!.Name!, cancellationToken);

            return response.ResultType switch
            {
                ResultType.Ok => Results.NoContent(),
                ResultType.NotFound => Results.NotFound(response.Message),
                ResultType.Conflict => Results.Conflict(response.Message),
                ResultType.Problems => Results.ValidationProblem(response.ValidationErrors!),
                _ => Results.Problem(response.Message)
            };
        })
        .WithName("UpdateApp")
        .WithTags("Apps")
        .RequireAuthorization();

        return app;
    }
}
