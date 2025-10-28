using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Employees.CreateEmployee;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Employees.GetCurrentEmployee;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Employees.GetEmployees;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Employees;

public static class DI
{
    public static IServiceCollection AddEmployeesServices(this IServiceCollection services)
    {
        services.AddTransient<ICreateEmployeeService, CreateEmployeeService>();
        services.AddTransient<IGetEmployeesService, GetEmployeesService>();
        services.AddTransient<IGetCurrentEmployeeService, GetCurrentEmployeeService>();

        return services;
    }
    public static WebApplication MapEmployeesEndpoints(this WebApplication app)
    {

        app.MapGetEmployeesEndPoint();

        app.MapGetCurrentEmployeeEndPoint();

        return app;
    }
}
