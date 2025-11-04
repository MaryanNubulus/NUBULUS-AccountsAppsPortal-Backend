using Microsoft.AspNetCore.Mvc;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Enums;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Employees.GetEmployees;

public static class GetEmployeesEndPoint
{
    public static WebApplication MapGetEmployeesEndPoint(this WebApplication app)
    {
        app.MapGet("/api/v1/employees", async ([FromServices] IGetEmployeesService getEmployeesService) =>
        {
            var response = await getEmployeesService.ExecuteAsync();

            return response.ResultType switch
            {
                ResultType.Ok => Results.Ok(response.Data),
                _ => Results.Problem(response.Message)
            };

        }).RequireAuthorization();

        return app;
    }
}