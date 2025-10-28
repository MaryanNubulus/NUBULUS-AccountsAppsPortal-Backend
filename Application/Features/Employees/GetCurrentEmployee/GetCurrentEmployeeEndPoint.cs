using Microsoft.AspNetCore.Mvc;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Enums;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Employees.GetCurrentEmployee;

public static class GetCurrentEmployeeEndPoint
{
    public static WebApplication MapGetCurrentEmployeeEndPoint(this WebApplication app)
    {
        app.MapGet("/api/v1/employees/current", async (HttpContext context,
            [FromServices] IGetCurrentEmployeeService getCurrentEmployeeService) =>
        {

            var employeeInfo = await getCurrentEmployeeService.GetCurrentEmployeeAsync(context.User.Identities.FirstOrDefault()!.Name!);
            var result = getCurrentEmployeeService.ResultType;

            if (result == ResultType.Error)
            {
                return Results.Problem(getCurrentEmployeeService.Message);
            }

            if (result == ResultType.Problems)
            {
                return Results.ValidationProblem(getCurrentEmployeeService.ValidationErrors);
            }

            if (result == ResultType.NotFound)
            {
                return Results.NotFound();
            }

            return Results.Ok(employeeInfo);

        })
        .RequireAuthorization();

        return app;
    }
}