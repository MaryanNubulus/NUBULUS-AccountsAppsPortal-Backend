using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Enums;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Users.GetUsers;

public static class GetUsersEndpoint
{
    public static WebApplication MapGetUsersEndpoint(this WebApplication app)
    {
        app.MapGet("/api/v1/accounts/{accountId}/users", async (
            [FromRoute] string accountId,
            [FromServices] IGetUsersService getUsersService) =>
        {
            var response = await getUsersService.ExecuteAsync(accountId);

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
