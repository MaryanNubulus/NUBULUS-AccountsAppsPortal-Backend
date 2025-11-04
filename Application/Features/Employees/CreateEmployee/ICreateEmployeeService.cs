using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.DTOs;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Abstractions;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Employees.CreateEmployee;

public interface ICreateEmployeeService
{
    Task<IGenericResponse<EmployeeInfoDTO>> ExecuteAsync(string email, string name);
}
