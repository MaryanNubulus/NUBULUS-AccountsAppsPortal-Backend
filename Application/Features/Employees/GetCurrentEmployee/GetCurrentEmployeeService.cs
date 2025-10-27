using MongoDB.Driver.Linq;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Abstractions;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.DTOs;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Mappers;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Employees.GetCurrentEmployee;

public class GetCurrentEmployeeService : IGetCurrentEmployeeService
{
    private readonly IEmployeesQueriesRepository _employeesQueriesRepository;

    public GetCurrentEmployeeService(IEmployeesQueriesRepository employeesQueriesRepository)
    {
        _employeesQueriesRepository = employeesQueriesRepository;
    }

    public async Task<EmployeeInfoDTO?> GetCurrentEmployeeAsync(string email)
    {
        var employee = await _employeesQueriesRepository.GetAll().FirstOrDefaultAsync(x => x.Email == email);
        if (employee == null) return null;
        return EmployeeMapper.ToDTO(employee);
    }
}
