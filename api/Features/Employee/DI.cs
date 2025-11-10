using Nubulus.Backend.Api.Features.Employees.GetCurrentEmployee;

namespace Nubulus.Backend.Api.Features.Employees;

public static class DI
{
    public static IServiceCollection AddEmployeeFeature(this IServiceCollection services)
    {
        return services;
    }
    public static WebApplication MapEmployeeEndpoints(this WebApplication app)
    {
        app.MapGetCurrentEmployeeEndPoint();
        return app;
    }
}