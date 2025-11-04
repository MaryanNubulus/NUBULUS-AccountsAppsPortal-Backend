using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Enums;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Accounts.GetAccounts;

public static class GetAccountsEndpoint
{
    public static WebApplication MapGetAccountsEndpoint(this WebApplication app)
    {
        app.MapGet("/api/v1/accounts", async ([FromServices] IGetAccountsService getAccountsService) =>
        {
            var response = await getAccountsService.ExecuteAsync();

            return response.ResultType switch
            {
                ResultType.Ok => Results.Ok(response.Data),
                _ => Results.Problem(response.Message)
            };
        }).RequireAuthorization();

        return app;
    }
}
