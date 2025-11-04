using MongoDB.Driver.Linq;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.DTOs;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Enums;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Abstractions;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Employees.Common;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Employees.GetEmployees;

internal sealed class GetEmployeesResponse : IGenericResponse<IEnumerable<EmployeeInfoDTO>>
{
    public ResultType ResultType { get; init; }
    public string? Message { get; init; }
    public Dictionary<string, string[]>? ValidationErrors { get; init; }
    public IEnumerable<EmployeeInfoDTO>? Data { get; init; }

    public static IGenericResponse<IEnumerable<EmployeeInfoDTO>> Success(IEnumerable<EmployeeInfoDTO> data) => new GetEmployeesResponse
    {
        ResultType = ResultType.Ok,
        Data = data
    };

    public static IGenericResponse<IEnumerable<EmployeeInfoDTO>> Error(string message) => new GetEmployeesResponse
    {
        ResultType = ResultType.Error,
        Message = message,
        Data = Enumerable.Empty<EmployeeInfoDTO>()
    };
}

public class GetEmployeesService : IGetEmployeesService
{
    private readonly IEmployeesQueriesRepository _employeesQueriesRepository;

    public GetEmployeesService(IEmployeesQueriesRepository employeesQueriesRepository)
    {
        _employeesQueriesRepository = employeesQueriesRepository;
    }

    public async Task<IGenericResponse<IEnumerable<EmployeeInfoDTO>>> ExecuteAsync()
    {
        try
        {
            var employeeEntities = await _employeesQueriesRepository.GetAll().ToListAsync();
            var employees = employeeEntities.Select(EmployeeMapper.ToDTO).ToList();
            return GetEmployeesResponse.Success(employees);
        }
        catch (Exception ex)
        {
            return GetEmployeesResponse.Error($"An error occurred while retrieving employees: {ex.Message}");
        }
    }
}