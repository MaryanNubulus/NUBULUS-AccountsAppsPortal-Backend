using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Enums;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Requests;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Users.CreateUser;

public static class CreateUserEndpoint
{
    public static WebApplication MapCreateUserEndpoint(this WebApplication app)
    {
        app.MapPost("/api/v1/accounts/{accountId}/users", async (
            [FromRoute] string accountId,
            [FromServices] ICreateUserService createUserService,
            [FromBody] CreateUserRequest request) =>
        {
            // Agregar el accountId del path al request
            var requestWithAccountId = request with { AccountId = accountId };

            var response = await createUserService.ExecuteAsync(requestWithAccountId);

            return response.ResultType switch
            {
                ResultType.Ok => Results.Created($"/api/v1/accounts/{accountId}/users/{response.Data?.Id}", response.Data),
                ResultType.Conflict => Results.Conflict(new { response.Message }),
                ResultType.Problems => Results.ValidationProblem(response.ValidationErrors!),
                _ => Results.Problem(response.Message)
            };
        }).RequireAuthorization();

        return app;
    }
}
