using Microsoft.AspNetCore.Mvc;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.DTOs;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Employees.GetEmployees;

public static class GetEmployeesEndPoint
{
    public static WebApplication MapGetEmployeesEndPoint(this WebApplication app)
    {
        app.MapGet("/api/v1/employees", async ([FromServices] IGetEmployeesService getEmployeesService) =>
        {
            var employees = await getEmployeesService.GetEmployeesAsync();
            var response = new GetEmployeesResponse();

            if (employees == null || !employees.Any())
            {
                response.Success = false;
                response.Message = "No employees found.";
                return Results.Ok(response);
            }
            response.Success = true;
            response.Employees = employees.ToList();

            return Results.Ok(response);
        }).RequireAuthorization();

        return app;
    }
}