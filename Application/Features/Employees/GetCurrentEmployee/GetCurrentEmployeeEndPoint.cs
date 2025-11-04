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
            var response = await getCurrentEmployeeService.ExecuteAsync(context.User.Identities.FirstOrDefault()!.Name!);

            return response.ResultType switch
            {
                ResultType.Ok => Results.Ok(response.Data),
                ResultType.NotFound => Results.NotFound(new { response.Message }),
                ResultType.Problems => Results.ValidationProblem(response.ValidationErrors!),
                _ => Results.Problem(response.Message)
            };

        })
        .RequireAuthorization();

        return app;
    }
}