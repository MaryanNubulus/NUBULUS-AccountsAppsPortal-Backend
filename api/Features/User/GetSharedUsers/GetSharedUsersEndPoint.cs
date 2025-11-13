using Microsoft.AspNetCore.Mvc;
using Nubulus.Backend.Api.Features.Common;

namespace Nubulus.Backend.Api.Features.User.GetSharedUsers;

public static class GetSharedUsersEndPoint
{
    public static WebApplication MapGetSharedUsersEndPoint(this WebApplication app)
    {
        app.MapGet("/api/v1/accounts/{accountId}/users/shareds", async (
            [FromRoute] int accountId,
            [FromQuery] string? searchTerm,
            [FromQuery] int? pageNumber,
            [FromQuery] int? pageSize,
            [FromServices] GetSharedUsersService service,
            CancellationToken cancellationToken) =>
        {
            var request = new GetSharedUsersRequest
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
        .WithName("GetSharedUsers")
        .WithTags("Users");

        return app;
    }
}
