using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Enums;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.DTOs;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Employees.GetCurrentEmployee;

public interface IGetCurrentEmployeeService
{
    Task<EmployeeInfoDTO?> GetCurrentEmployeeAsync(string email);

    ResultType ResultType { get; }

    string? Message { get; }

    Dictionary<string, string[]> ValidationErrors { get; }
}
