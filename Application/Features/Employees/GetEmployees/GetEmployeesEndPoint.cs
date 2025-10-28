using Microsoft.AspNetCore.Mvc;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Enums;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Responses;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Employees.GetEmployees;

public static class GetEmployeesEndPoint
{
    public static WebApplication MapGetEmployeesEndPoint(this WebApplication app)
    {
        app.MapGet("/api/v1/employees", async ([FromServices] IGetEmployeesService getEmployeesService) =>
        {
            var employeesList = await getEmployeesService.GetEmployeesAsync();
            var result = getEmployeesService.ResultType;

            if (result == ResultType.Error)
            {
                return Results.Problem(getEmployeesService.Message);
            }

            return Results.Ok(new GetEmployeesResponse(employeesList.ToList()));

        }).RequireAuthorization();

        return app;
    }
}