using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.DTOs;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Employees.GetEmployees;

public interface IGetEmployeesService
{
    Task<IEnumerable<EmployeeInfoDTO>> GetEmployeesAsync();
}
