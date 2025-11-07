using Microsoft.AspNetCore.Mvc;
using Nubulus.Backend.Api.Features.Common;

namespace Nubulus.Backend.Api.Features.Account.GetAccounts;

public static class GetAccountsEndPoint
{
    public static WebApplication MapGetAccountsEndPoint(this WebApplication app)
    {
        app.MapGet("/api/v1/accounts", async (
            [FromQuery] int pageNumber,
            [FromQuery] int pageSize,
            GetAccountsService service,
            CancellationToken cancellationToken) =>
        {
            var request = new PaginatedRequest
            {
                PageNumber = pageNumber > 0 ? pageNumber : 1,
                PageSize = pageSize > 0 ? pageSize : 10
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
        .WithName("GetAccounts")
        .WithTags("Accounts");

        return app;
    }
}
