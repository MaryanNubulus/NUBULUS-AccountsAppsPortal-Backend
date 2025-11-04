using MongoDB.Driver.Linq;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.DTOs;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Enums;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Requests;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Abstractions;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Employees.Common;
using NUBULUS.AccountsAppsPortalBackEnd.Domain.Entities;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Employees.CreateEmployee;

internal sealed class CreateEmployeeResponse : IGenericResponse<EmployeeInfoDTO>
{
    public ResultType ResultType { get; init; }
    public string? Message { get; init; }
    public Dictionary<string, string[]>? ValidationErrors { get; init; }
    public EmployeeInfoDTO? Data { get; init; }

    public static IGenericResponse<EmployeeInfoDTO> Success(EmployeeInfoDTO data, string message) => new CreateEmployeeResponse
    {
        ResultType = ResultType.Ok,
        Data = data,
        Message = message
    };

    public static IGenericResponse<EmployeeInfoDTO> AlreadyExists(EmployeeInfoDTO data, string message) => new CreateEmployeeResponse
    {
        ResultType = ResultType.Ok,
        Data = data,
        Message = message
    };

    public static IGenericResponse<EmployeeInfoDTO> Error(string message) => new CreateEmployeeResponse
    {
        ResultType = ResultType.Error,
        Message = message
    };

    public static IGenericResponse<EmployeeInfoDTO> ValidationError(string field, string message) => new CreateEmployeeResponse
    {
        ResultType = ResultType.Problems,
        ValidationErrors = new Dictionary<string, string[]> { { field, new[] { message } } }
    };
}

public class CreateEmployeeService : ICreateEmployeeService
{
    private readonly IEmployeesCommandsRepository _employeesCommandsRepository;
    private readonly IEmployeesQueriesRepository _employeesQueriesRepository;

    public CreateEmployeeService(IEmployeesCommandsRepository employeesCommandsRepository, IEmployeesQueriesRepository employeesQueriesRepository)
    {
        _employeesCommandsRepository = employeesCommandsRepository;
        _employeesQueriesRepository = employeesQueriesRepository;
    }

    public async Task<IGenericResponse<EmployeeInfoDTO>> ExecuteAsync(string email, string name)
    {
        CreateEmployeeRequest request;
        try
        {
            request = CreateEmployeeRequest.Create(email, name);
        }
        catch (Exception ex)
        {
            return CreateEmployeeResponse.ValidationError("Request", ex.Message);
        }

        var existingEmployee = await _employeesQueriesRepository.GetAll().FirstOrDefaultAsync(x => x.Email == request.Email.Value);

        if (existingEmployee != null)
        {
            var existingDto = EmployeeMapper.ToDTO(existingEmployee);
            return CreateEmployeeResponse.AlreadyExists(existingDto, "An employee exists.");
        }

        Employee employee;

        try
        {
            employee = EmployeeMapper.CreateEntity(request);
        }
        catch (Exception ex)
        {
            return CreateEmployeeResponse.Error($"Error creating employee entity: {ex.Message}");
        }

        try
        {
            await _employeesCommandsRepository.AddAsync(employee);
            var employeeDto = EmployeeMapper.ToDTO(employee);
            return CreateEmployeeResponse.Success(employeeDto, "Employee created successfully.");
        }
        catch (Exception)
        {
            return CreateEmployeeResponse.Error("Error saving employee to the database.");
        }
    }
}