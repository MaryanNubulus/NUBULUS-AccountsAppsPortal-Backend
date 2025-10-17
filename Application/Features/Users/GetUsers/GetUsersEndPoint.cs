using Microsoft.AspNetCore.Mvc;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.DTOs;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Users.GetUsers;

public static class GetUsersEndPoint
{
    public static WebApplication MapGetUsersEndPoint(this WebApplication app)
    {
        app.MapGet("/api/v1/users", async ([FromServices] IGetUsersService getUsersService) =>
        {
            var users = await getUsersService.GetUsersAsync();
            var response = new GetUsersResponse();

            if (users == null || !users.Any())
            {
                response.Success = false;
                response.Message = "No users found.";
                return Results.Ok(response);
            }
            response.Success = true;
            response.Users = users.ToList();

            return Results.Ok(response);
        }).RequireAuthorization();

        return app;
    }
}