using Microsoft.AspNetCore.Mvc;
using Nubulus.Backend.Api.Features.Common;

namespace Nubulus.Backend.Api.Features.User.GetUser;

public static class GetUserEndPoint
{
    public static WebApplication MapGetUserEndPoint(this WebApplication app)
    {
        app.MapGet(GetUserRequest.Route, async (
            int accountId,
            int userId,
            [FromServices] GetUserService service,
            CancellationToken cancellationToken) =>
        {
            var request = new GetUserRequest { AccountId = accountId, UserId = userId };
            var response = await service.ExecuteAsync(request, cancellationToken);
            return response.ResultType switch
            {
                ResultType.Ok => Results.Ok(response.Data),
                ResultType.NotFound => Results.NotFound(response.Message),
                _ => Results.Problem(response.Message)
            };
        })
        .WithName("GetUser")
        .WithTags("Users")
        .RequireAuthorization();

        return app;
    }
}
