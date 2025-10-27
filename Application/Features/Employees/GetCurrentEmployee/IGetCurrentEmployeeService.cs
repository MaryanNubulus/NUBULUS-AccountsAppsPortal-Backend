using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.DTOs;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Employees.GetCurrentEmployee;

public interface IGetCurrentEmployeeService
{
    Task<EmployeeInfoDTO?> GetCurrentEmployeeAsync(string email);
}
