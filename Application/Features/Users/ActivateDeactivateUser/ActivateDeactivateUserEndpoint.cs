using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Enums;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Users.ActivateDeactivateUser;

public static class ActivateDeactivateUserEndpoint
{
    public static WebApplication MapActivateDeactivateUserEndpoint(this WebApplication app)
    {
        app.MapPost("/api/v1/accounts/{accountId}/users/{userId}/activate", async (
            [FromRoute] string accountId,
            [FromRoute] string userId,
            [FromServices] IActivateDeactivateUserService service) =>
        {
            var response = await service.ExecuteAsync(accountId, userId, activate: true);

            return response.ResultType switch
            {
                ResultType.Ok => Results.Ok(response.Data),
                ResultType.Problems => Results.ValidationProblem(response.ValidationErrors!),
                _ => Results.Problem(response.Message)
            };
        }).RequireAuthorization();

        app.MapPost("/api/v1/accounts/{accountId}/users/{userId}/deactivate", async (
            [FromRoute] string accountId,
            [FromRoute] string userId,
            [FromServices] IActivateDeactivateUserService service) =>
        {
            var response = await service.ExecuteAsync(accountId, userId, activate: false);

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
