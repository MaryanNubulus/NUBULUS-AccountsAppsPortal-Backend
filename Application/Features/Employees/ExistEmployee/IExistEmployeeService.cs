namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Employees.ExistEmployee;

public interface IExistEmployeeService
{
    Task<bool> ExistEmployeeAsync(string email);
}
