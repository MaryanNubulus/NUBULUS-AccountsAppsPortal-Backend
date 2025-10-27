using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Abstractions;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.DTOs;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Mappers;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Employees.CreateEmployee;

public class CreateEmployeeService : ICreateEmployeeService
{
    private readonly IEmployeesCommandsRepository _employeesCommandsRepository;

    public CreateEmployeeService(IEmployeesCommandsRepository employeesCommandsRepository)
    {
        _employeesCommandsRepository = employeesCommandsRepository;
    }

    public async Task<bool> CreateEmployeeAsync(CreateEmployeeRequest request)
    {
        var employee = EmployeeMapper.CreateEntity(request);

        return await _employeesCommandsRepository.AddAsync(employee);
    }
}