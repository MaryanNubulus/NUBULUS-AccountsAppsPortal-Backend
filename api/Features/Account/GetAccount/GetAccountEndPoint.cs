using Microsoft.AspNetCore.Mvc;
using Nubulus.Backend.Api.Features.Common;

namespace Nubulus.Backend.Api.Features.Account.GetAccount;

public static class GetAccountEndPoint
{
    public static WebApplication MapGetAccountEndPoint(this WebApplication app)
    {
        app.MapGet(GetAccountRequest.Route, async ([AsParameters] GetAccountRequest request, [FromServices] GetAccountService service, CancellationToken cancellationToken) =>
        {
            var response = await service.ExecuteAsync(request, cancellationToken);
            return response.ResultType switch
            {
                ResultType.Ok => Results.Ok(response.Data),
                ResultType.NotFound => Results.NotFound(response.Message),
                ResultType.Problems => Results.ValidationProblem(response.ValidationErrors!),
                _ => Results.Problem(response.Message)
            };
        })
        .WithName("GetAccount")
        .WithTags("Accounts")
        .RequireAuthorization();

        return app;
    }
}