using Microsoft.AspNetCore.Mvc;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Employees.GetCurrentEmployee;

public static class GetCurrentEmployeeEndPoint
{
    public static WebApplication MapGetCurrentEmployeeEndPoint(this WebApplication app)
    {
        app.MapGet("/api/v1/employees/current", async (HttpContext context,
            [FromServices] IGetCurrentEmployeeService getCurrentEmployeeService) =>
        {
            var email = context.User.Identities.FirstOrDefault()!.Name!;
            var employeeInfo = await getCurrentEmployeeService.GetCurrentEmployeeAsync(email);
            return employeeInfo != null
                ? Results.Ok(employeeInfo)
                : Results.NotFound();
        }).RequireAuthorization();

        return app;
    }
}