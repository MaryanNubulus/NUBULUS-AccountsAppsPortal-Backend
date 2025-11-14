using Microsoft.AspNetCore.Mvc;
using Nubulus.Backend.Api.Features.Common;

namespace Nubulus.Backend.Api.Features.App.GetApp;

public static class GetAppEndPoint
{
    public static WebApplication MapGetAppEndPoint(this WebApplication app)
    {
        app.MapGet(GetAppRequest.Route, async (
            [FromRoute] int id,
            [FromServices] GetAppService service,
            CancellationToken cancellationToken) =>
        {
            var request = new GetAppRequest { Id = id };
            var response = await service.ExecuteAsync(request, cancellationToken);

            return response.ResultType switch
            {
                ResultType.Ok => Results.Ok(response.Data),
                ResultType.NotFound => Results.NotFound(response.Message),
                _ => Results.Problem(response.Message)
            };
        })
        .WithName("GetApp")
        .WithTags("Apps")
        .RequireAuthorization();

        return app;
    }
}
