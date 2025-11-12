using Microsoft.AspNetCore.Mvc;
using Nubulus.Backend.Api.Features.Common;

namespace Nubulus.Backend.Api.Features.User.GetUsers;

public static class GetUsersEndPoint
{
    public static WebApplication MapGetUsersEndPoint(this WebApplication app)
    {
        app.MapGet(GetUsersRequest.Route, async (
            int accountId,
            [AsParameters] GetUsersRequest request,
            [FromServices] GetUsersService service,
            CancellationToken cancellationToken) =>
        {
            request.AccountId = accountId;
            var response = await service.ExecuteAsync(request, cancellationToken);
            return response.ResultType switch
            {
                ResultType.Ok => Results.Ok(response.Data),
                ResultType.NotFound => Results.NotFound(response.Message),
                _ => Results.Problem(response.Message)
            };
        })
        .WithName("GetUsers")
        .WithTags("Users")
        .RequireAuthorization();

        return app;
    }
}
