using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.DTOs;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Enums;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Employees.GetEmployees;

public interface IGetEmployeesService
{
    Task<IEnumerable<EmployeeInfoDTO>> GetEmployeesAsync();

    ResultType ResultType { get; }

    string? Message { get; }
}
