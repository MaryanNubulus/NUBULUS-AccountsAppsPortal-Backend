using MongoDB.Driver.Linq;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Abstractions;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Employees.ExistEmployee;

public class ExistEmployeeService : IExistEmployeeService
{
    private readonly IEmployeesQueriesRepository _employeesQueriesRepository;

    public ExistEmployeeService(IEmployeesQueriesRepository employeesQueriesRepository)
    {
        _employeesQueriesRepository = employeesQueriesRepository;
    }

    public async Task<bool> ExistEmployeeAsync(string email)
    {
        return await _employeesQueriesRepository.GetAll().AnyAsync(x => x.Email == email);
    }

}
