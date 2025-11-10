namespace Nubulus.Backend.Api.Features.Employees.GetCurrentEmployee;

public static class GetCurrentEmployeeEndPoint
{
    public static WebApplication MapGetCurrentEmployeeEndPoint(this WebApplication app)
    {
        app.MapGet("/api/v1/employees/current", (HttpContext context) =>
        {
            var employeeEmail = context.User.Identities.FirstOrDefault()!.Name!;
            var employeeName = context.User.Claims.FirstOrDefault(c => c.Type == "name")?.Value ?? "Unknown";

            return Results.Ok(new
            {
                Id = Guid.NewGuid(),
                Email = employeeEmail,
                Name = employeeName,
                IsActive = true
            });

        })
        .WithName("GetCurrentEmployee")
        .WithTags("Employees")
        .RequireAuthorization();

        return app;
    }
}