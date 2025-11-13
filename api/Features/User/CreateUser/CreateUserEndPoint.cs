using Microsoft.AspNetCore.Mvc;
using Nubulus.Backend.Api.Features.Common;

namespace Nubulus.Backend.Api.Features.User.CreateUser;

public static class CreateUserEndPoint
{
    public static WebApplication MapCreateUserEndPoint(this WebApplication app)
    {
        app.MapPost(CreateUserRequest.Route, async (
            HttpContext context,
            [FromRoute] int accountId,
            [FromBody] CreateUserRequest request,
            [FromServices] CreateUserService service,
            CancellationToken cancellationToken) =>
        {
            request.AccountId = accountId;
            var response = await service.ExecuteAsync(request, context.User.Identities.FirstOrDefault()!.Name!, cancellationToken);
            return response.ResultType switch
            {
                ResultType.Ok => Results.Created(),
                ResultType.Conflict => Results.Conflict(response.Message),
                ResultType.NotFound => Results.NotFound(response.Message),
                ResultType.Problems => Results.ValidationProblem(response.ValidationErrors!),
                _ => Results.Problem(response.Message)
            };
        })
        .WithName("CreateUser")
        .WithTags("Users")
        .RequireAuthorization();

        return app;
    }
}
