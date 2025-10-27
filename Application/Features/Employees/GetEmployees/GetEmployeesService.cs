using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Abstractions;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.DTOs;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Mappers;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Employees.GetEmployees;

public class GetEmployeesService : IGetEmployeesService
{
    private readonly IEmployeesQueriesRepository _employeesQueriesRepository;

    public GetEmployeesService(IEmployeesQueriesRepository employeesQueriesRepository)
    {
        _employeesQueriesRepository = employeesQueriesRepository;
    }

    public async Task<IEnumerable<EmployeeInfoDTO>> GetEmployeesAsync()
    {
        return await Task.FromResult(_employeesQueriesRepository.GetAll().Select(EmployeeMapper.ToDTO).ToList());

    }
}