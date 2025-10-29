using MongoDB.Driver.Linq;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.DTOs;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Enums;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Abstractions;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Employees.Common;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Employees.GetEmployees;

public class GetEmployeesService : IGetEmployeesService
{
    private readonly IEmployeesQueriesRepository _employeesQueriesRepository;

    public GetEmployeesService(IEmployeesQueriesRepository employeesQueriesRepository)
    {
        _employeesQueriesRepository = employeesQueriesRepository;
    }

    public ResultType ResultType { get; private set; } = ResultType.None;

    public string? Message { get; private set; }

    public async Task<IEnumerable<EmployeeInfoDTO>> GetEmployeesAsync()
    {
        List<EmployeeInfoDTO> employees = new();

        try
        {
            var employeeEntities = await _employeesQueriesRepository.GetAll().ToListAsync();
            employees = employeeEntities.Select(EmployeeMapper.ToDTO).ToList();
        }
        catch (Exception ex)
        {
            ResultType = ResultType.Error;
            Message = $"An error occurred while retrieving employees: {ex.Message}";
        }

        if (employees.Count == 0)
        {
            ResultType = ResultType.Ok;
            Message = "No employees found.";
        }
        else
        {
            ResultType = ResultType.Ok;
            Message = $"{employees.Count} employees found.";
        }

        return employees;
    }
}