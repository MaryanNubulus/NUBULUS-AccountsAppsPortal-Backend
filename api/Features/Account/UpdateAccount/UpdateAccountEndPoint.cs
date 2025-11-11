using Microsoft.AspNetCore.Mvc;
using Nubulus.Backend.Api.Features.Common;

namespace Nubulus.Backend.Api.Features.Account.UpdateAccount;

public static class UpdateAccountEndPoint
{
    public static WebApplication MapUpdateAccountEndPoint(this WebApplication app)
    {
        app.MapPut(UpdateAccountRequest.Route, async (HttpContext httpContext, [FromRoute] string accountKey, [FromBody] UpdateAccountRequest request, [FromServices] UpdateAccountService service, CancellationToken cancellationToken) =>
        {
            var response = await service.ExecuteAsync(accountKey, request, httpContext.User.Identity!.Name!, cancellationToken);
            return response.ResultType switch
            {
                ResultType.Ok => Results.Ok(),
                ResultType.NotFound => Results.NotFound(response.Message),
                ResultType.Conflict => Results.Conflict(response.Message),
                ResultType.Problems => Results.ValidationProblem(response.ValidationErrors!),
                _ => Results.Problem(response.Message)
            };
        })
        .WithName("UpdateAccount")
        .WithTags("Accounts")
        .RequireAuthorization();

        return app;
    }
}