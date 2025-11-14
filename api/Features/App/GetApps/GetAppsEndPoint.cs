using Microsoft.AspNetCore.Mvc;
using Nubulus.Backend.Api.Features.Common;

namespace Nubulus.Backend.Api.Features.App.GetApps;

public static class GetAppsEndPoint
{
    public static WebApplication MapGetAppsEndPoint(this WebApplication app)
    {
        app.MapGet(GetAppsRequest.Route, async (
            [FromQuery] int? pageNumber,
            [FromQuery] int? pageSize,
            [FromQuery] string? searchTerm,
            [FromServices] GetAppsService service,
            CancellationToken cancellationToken) =>
        {
            var request = new GetAppsRequest
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SearchTerm = searchTerm
            };

            var response = await service.ExecuteAsync(request, cancellationToken);

            return response.ResultType switch
            {
                ResultType.Ok => Results.Ok(response.Data),
                _ => Results.Problem(response.Message)
            };
        })
        .WithName("GetApps")
        .WithTags("Apps")
        .RequireAuthorization();

        return app;
    }
}
