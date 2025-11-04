using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Enums;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Requests;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.ValueObjects;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Accounts.UpdateAccount;

public static class UpdateAccountEndpoint
{
    public static WebApplication MapUpdateAccountEndpoint(this WebApplication app)
    {
        app.MapPut("/api/v1/accounts/{accountId:guid}", async (
            [FromServices] IUpdateAccountService updateAccountService,
            [FromRoute] Guid accountId,
            [FromBody] UpdateAccountRequest request) =>
        {
            var accountIdVO = IdObject.Create(accountId);
            var response = await updateAccountService.ExecuteAsync(accountIdVO, request);

            return response.ResultType switch
            {
                ResultType.Ok => Results.Ok(response.Data),
                ResultType.Conflict => Results.Conflict(new { response.Message }),
                ResultType.Problems => Results.ValidationProblem(response.ValidationErrors!),
                _ => Results.Problem(response.Message)
            };
        }).RequireAuthorization();

        return app;
    }
}
