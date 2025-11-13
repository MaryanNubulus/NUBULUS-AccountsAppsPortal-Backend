using Microsoft.AspNetCore.Mvc;
using Nubulus.Backend.Api.Features.Common;

namespace Nubulus.Backend.Api.Features.User.UpdateUser;

public static class UpdateUserEndPoint
{
    public static WebApplication MapUpdateUserEndPoint(this WebApplication app)
    {
        app.MapPut(UpdateUserRequest.Route, async (
            HttpContext context,
            [FromRoute] int accountId,
            [FromRoute] int userId,
            [FromBody] UpdateUserRequest request,
            [FromServices] UpdateUserService service,
            CancellationToken cancellationToken) =>
        {
            request.AccountId = accountId;
            var response = await service.ExecuteAsync(userId, request, context.User.Identities.FirstOrDefault()!.Name!, cancellationToken);
            return response.ResultType switch
            {
                ResultType.Ok => Results.Ok(new { data = response.Data }),
                ResultType.NotFound => Results.NotFound(response.Message),
                ResultType.Conflict => Results.Conflict(response.Message),
                ResultType.Problems => Results.ValidationProblem(response.ValidationErrors!),
                _ => Results.Problem(response.Message)
            };
        })
        .WithName("UpdateUser")
        .WithTags("Users")
        .RequireAuthorization();

        return app;
    }
}
