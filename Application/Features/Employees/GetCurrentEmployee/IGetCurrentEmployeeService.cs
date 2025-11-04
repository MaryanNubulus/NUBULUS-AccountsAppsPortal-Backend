using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.DTOs;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Abstractions;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Employees.GetCurrentEmployee;

public interface IGetCurrentEmployeeService
{
    Task<IGenericResponse<EmployeeInfoDTO>> ExecuteAsync(string email);
}
