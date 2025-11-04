using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.DTOs;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Abstractions;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Employees.GetEmployees;

public interface IGetEmployeesService
{
    Task<IGenericResponse<IEnumerable<EmployeeInfoDTO>>> ExecuteAsync();
}
