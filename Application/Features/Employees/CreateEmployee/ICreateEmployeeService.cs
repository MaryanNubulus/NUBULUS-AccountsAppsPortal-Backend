using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.DTOs;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Employees.CreateEmployee;

public interface ICreateEmployeeService
{
    Task<bool> CreateEmployeeAsync(CreateEmployeeRequest request);
}
