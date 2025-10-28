using Microsoft.AspNetCore.Mvc;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Enums;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Employees.CreateEmployee;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Auth;

public static class SignInEndpoint
{
    public static WebApplication MapSignInEndpoint(this WebApplication app)
    {
        app.MapGet("/api/v1/auth/sign-in", () =>
        {
            return Results.Redirect("/api/v1/auth/success");
        }).RequireAuthorization();

        app.MapGet("/api/v1/auth/success", async (HttpContext context, [FromServices] ICreateEmployeeService createEmployeeService) =>
        {
            var employeeEmail = context.User.Identities.FirstOrDefault()!.Name!;
            var employeeName = context.User.Claims.FirstOrDefault(c => c.Type == "name")?.Value ?? "Unknown";

            await createEmployeeService.CreateEmployeeAsync(employeeEmail, employeeName);
            var result = createEmployeeService.ResultType;

            if (result == ResultType.Error)
            {
                return Results.Problem(createEmployeeService.Message);
            }

            if (result == ResultType.Problems)
            {
                return Results.ValidationProblem(createEmployeeService.ValidationErrors);
            }

            return Results.Redirect("http://localhost:5173/private");

        }).RequireAuthorization();

        return app;
    }
}
