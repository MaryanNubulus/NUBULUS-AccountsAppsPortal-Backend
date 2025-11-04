using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Enums;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.ValueObjects;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Accounts.DesactivateActivateAccount;

public static class DesactivateActivateAccountEndpoint
{
    public static WebApplication MapDesactivateActivateAccountEndpoint(this WebApplication app)
    {
        app.MapPost("/api/v1/accounts/{accountId:guid}/deactivate", async (
            [FromServices] IDesactivateActivateAccountService service,
            [FromRoute] Guid accountId) =>
        {
            var accountIdVO = IdObject.Create(accountId);
            var response = await service.ExecuteAsync(accountIdVO, false);

            return response.ResultType switch
            {
                ResultType.Ok => Results.Ok(response.Data),
                ResultType.Problems => Results.ValidationProblem(response.ValidationErrors!),
                _ => Results.Problem(response.Message)
            };
        }).RequireAuthorization();

        app.MapPost("/api/v1/accounts/{accountId:guid}/activate", async (
            [FromServices] IDesactivateActivateAccountService service,
            [FromRoute] Guid accountId) =>
        {
            var accountIdVO = IdObject.Create(accountId);
            var response = await service.ExecuteAsync(accountIdVO, true);

            return response.ResultType switch
            {
                ResultType.Ok => Results.Ok(response.Data),
                ResultType.Problems => Results.ValidationProblem(response.ValidationErrors!),
                _ => Results.Problem(response.Message)
            };
        }).RequireAuthorization();

        return app;
    }
}
