using Microsoft.AspNetCore.Mvc;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.DTOs;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Employees.CreateEmployee;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Employees.ExistEmployee;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Employees.Auth;

public static class SignInEndpoint
{
    public static WebApplication MapSignInEndpoint(this WebApplication app)
    {
        app.MapGet("/api/v1/auth/sign-in", () =>
        {
            return Results.Redirect("/api/v1/auth/success");
        }).RequireAuthorization();

        app.MapGet("/api/v1/auth/success", async (HttpContext context, [FromServices] IExistEmployeeService existEmployeeService, [FromServices] ICreateEmployeeService createEmployeeService) =>
        {
            var employeeEmail = context.User.Identities.FirstOrDefault()!.Name!;
            var employeeExists = await existEmployeeService.ExistEmployeeAsync(employeeEmail);

            if (!employeeExists)
            {
                var employeeName = context.User.Claims.FirstOrDefault(c => c.Type == "name")?.Value ?? "Unknown";

                var request = new CreateEmployeeRequest
                {
                    Email = employeeEmail,
                    Name = employeeName
                };
                var created = await createEmployeeService.CreateEmployeeAsync(request);
                if (!created)
                {
                    return Results.StatusCode(500);
                }
            }
            return Results.Redirect("http://localhost:5173/private");
        }).RequireAuthorization();

        return app;
    }
}

