using Microsoft.AspNetCore.Mvc;
using Nubulus.Backend.Api.Features.Common;

namespace Nubulus.Backend.Api.Features.User.GetUsersToShare;

public static class GetUsersToShareEndPoint
{
    public static WebApplication MapGetUsersToShareEndPoint(this WebApplication app)
    {
        app.MapGet(GetUsersToShareRequest.Route, async (
            [FromRoute] int accountId,
            [FromQuery] string? searchTerm,
            [FromQuery] int? pageNumber,
            [FromQuery] int? pageSize,
            [FromServices] GetUsersToShareService service,
            CancellationToken cancellationToken) =>
        {
            var request = new GetUsersToShareRequest
            {
                AccountId = accountId,
                SearchTerm = searchTerm,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            var response = await service.ExecuteAsync(request, cancellationToken);
            return response.ResultType switch
            {
                ResultType.Ok => Results.Ok(response.Data),
                ResultType.NotFound => Results.NotFound(response.Message),
                ResultType.Problems => Results.ValidationProblem(response.ValidationErrors!),
                _ => Results.Problem(response.Message)
            };
        })
        .WithName("GetUsersToShare")
        .WithTags("Users");

        return app;
    }
}
