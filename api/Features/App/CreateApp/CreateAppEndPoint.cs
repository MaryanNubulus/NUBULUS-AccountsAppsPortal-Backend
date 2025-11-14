using Microsoft.AspNetCore.Mvc;
using Nubulus.Backend.Api.Features.Common;

namespace Nubulus.Backend.Api.Features.App.CreateApp;

public static class CreateAppEndPoint
{
    public static WebApplication MapCreateAppEndPoint(this WebApplication app)
    {
        app.MapPost(CreateAppRequest.Route, async (HttpContext context, [FromBody] CreateAppRequest request, [FromServices] CreateAppService service, CancellationToken cancellationToken) =>
        {
            var response = await service.ExecuteAsync(request, context.User.Identities.FirstOrDefault()!.Name!, cancellationToken);
            return response.ResultType switch
            {
                ResultType.Ok => Results.Created(),
                ResultType.Conflict => Results.Conflict(response.Message),
                ResultType.Problems => Results.ValidationProblem(response.ValidationErrors!),
                _ => Results.Problem(response.Message)
            };
        })
        .WithName("CreateApp")
        .WithTags("Apps")
        .RequireAuthorization();

        return app;
    }
}
