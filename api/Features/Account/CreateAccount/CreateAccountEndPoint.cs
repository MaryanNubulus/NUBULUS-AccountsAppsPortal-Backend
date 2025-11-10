using Microsoft.AspNetCore.Mvc;
using Nubulus.Backend.Api.Features.Account.GetAccount;
using Nubulus.Backend.Api.Features.Common;

namespace Nubulus.Backend.Api.Features.Account.CreateAccount;

public static class CreateAccountEndPoint
{
    public static WebApplication MapCreateAccountEndPoint(this WebApplication app)
    {
        app.MapPost(CreateAccountRequest.Route, async (HttpContext context, [FromBody] CreateAccountRequest request, [FromServices] CreateAccountService service, CancellationToken cancellationToken) =>
        {
            var userContext = context.User.Identities.FirstOrDefault()!.Name!;

            var response = await service.ExecuteAsync(request, userContext, cancellationToken);
            return response.ResultType switch
            {
                ResultType.Ok => Results.Created(GetAccountRequest.Route.Replace("{accountKey}", response.Data), null),
                ResultType.Conflict => Results.Conflict(response.Message),
                ResultType.Problems => Results.ValidationProblem(response.ValidationErrors!),
                _ => Results.Problem(response.Message)
            };
        })
        .WithName("CreateAccount")
        .WithTags("Accounts")
        .RequireAuthorization();

        return app;
    }
}