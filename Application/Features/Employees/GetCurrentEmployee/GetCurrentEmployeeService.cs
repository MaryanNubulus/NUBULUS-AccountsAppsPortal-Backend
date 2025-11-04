using MongoDB.Driver.Linq;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.DTOs;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Enums;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.ValueObjects;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Abstractions;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Employees.Common;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Employees.GetCurrentEmployee;

internal sealed class GetCurrentEmployeeResponse : IGenericResponse<EmployeeInfoDTO>
{
    public ResultType ResultType { get; init; }
    public string? Message { get; init; }
    public Dictionary<string, string[]>? ValidationErrors { get; init; }
    public EmployeeInfoDTO? Data { get; init; }

    public static IGenericResponse<EmployeeInfoDTO> Success(EmployeeInfoDTO data) => new GetCurrentEmployeeResponse
    {
        ResultType = ResultType.Ok,
        Data = data
    };

    public static IGenericResponse<EmployeeInfoDTO> NotFound(string message) => new GetCurrentEmployeeResponse
    {
        ResultType = ResultType.NotFound,
        Message = message
    };

    public static IGenericResponse<EmployeeInfoDTO> ValidationError(string field, string message) => new GetCurrentEmployeeResponse
    {
        ResultType = ResultType.Problems,
        ValidationErrors = new Dictionary<string, string[]> { { field, new[] { message } } }
    };
}

public class GetCurrentEmployeeService : IGetCurrentEmployeeService
{
    private readonly IEmployeesQueriesRepository _employeesQueriesRepository;

    public GetCurrentEmployeeService(IEmployeesQueriesRepository employeesQueriesRepository)
    {
        _employeesQueriesRepository = employeesQueriesRepository;
    }

    public async Task<IGenericResponse<EmployeeInfoDTO>> ExecuteAsync(string email)
    {
        Email emailObject;

        try
        {
            emailObject = Email.Create(email);
        }
        catch (Exception ex)
        {
            return GetCurrentEmployeeResponse.ValidationError("Email", ex.Message);
        }

        var employee = await _employeesQueriesRepository.GetAll().FirstOrDefaultAsync(x => x.Email == emailObject.Value);
        if (employee == null)
        {
            return GetCurrentEmployeeResponse.NotFound("Employee not found.");
        }

        var employeeDto = EmployeeMapper.ToDTO(employee);
        return GetCurrentEmployeeResponse.Success(employeeDto);
    }
}
