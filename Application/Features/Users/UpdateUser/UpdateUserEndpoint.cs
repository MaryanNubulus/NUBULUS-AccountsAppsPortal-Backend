using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Enums;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Requests;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Users.UpdateUser;

public static class UpdateUserEndpoint
{
    public static WebApplication MapUpdateUserEndpoint(this WebApplication app)
    {
        app.MapPut("/api/v1/accounts/{accountId}/users/{userId}", async (
            [FromRoute] string accountId,
            [FromRoute] string userId,
            [FromServices] IUpdateUserService updateUserService,
            [FromBody] UpdateUserRequest request) =>
        {
            var response = await updateUserService.ExecuteAsync(accountId, userId, request);

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
