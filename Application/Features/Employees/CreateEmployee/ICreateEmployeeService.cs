
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Enums;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Employees.CreateEmployee;

public interface ICreateEmployeeService
{
    Task CreateEmployeeAsync(string email, string name);

    ResultType ResultType { get; }

    string? Message { get; }

    Dictionary<string, string[]> ValidationErrors { get; }
}
