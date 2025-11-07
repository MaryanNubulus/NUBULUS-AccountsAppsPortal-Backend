using Microsoft.AspNetCore.Mvc;
using Nubulus.Backend.Api.Features.Common;

namespace Nubulus.Backend.Api.Features.Account.GetAccounts;

public static class GetAccountsEndPoint
{
    public static WebApplication MapGetAccountsEndPoint(this WebApplication app)
    {
        app.MapGet(GetAccountsRequest.Route, async ([AsParameters] GetAccountsRequest request, GetAccountsService service, CancellationToken cancellationToken) =>
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
        .WithName("GetAccounts")
        .WithTags("Accounts")
        .RequireAuthorization();

        return app;
    }
}
