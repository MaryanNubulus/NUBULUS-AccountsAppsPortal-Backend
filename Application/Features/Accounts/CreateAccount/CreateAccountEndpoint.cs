using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Enums;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Requests;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Accounts.CreateAccount;

public static class CreateAccountEndpoint
{
    public static WebApplication MapCreateAccountEndpoint(this WebApplication app)
    {
        app.MapPost("/api/v1/accounts", async ([FromServices] ICreateAccountService createAccountService,
                                             [FromBody] CreateAccountRequest request) =>
        {
            var response = await createAccountService.ExecuteAsync(request);

            return response.ResultType switch
            {
                ResultType.Ok => Results.Created($"/api/v1/accounts/{response.Data?.Id}", response.Data),
                ResultType.Conflict => Results.Conflict(new { response.Message }),
                ResultType.Problems => Results.ValidationProblem(response.ValidationErrors!),
                _ => Results.Problem(response.Message)
            };
        }).RequireAuthorization();

        return app;
    }
}