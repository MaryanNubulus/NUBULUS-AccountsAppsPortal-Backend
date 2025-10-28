using MongoDB.Driver.Linq;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Enums;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Requests;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Abstractions;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Mappers;
using NUBULUS.AccountsAppsPortalBackEnd.Domain.Entities;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Employees.CreateEmployee;

public class CreateEmployeeService : ICreateEmployeeService
{
    private readonly IEmployeesCommandsRepository _employeesCommandsRepository;
    private readonly IEmployeesQueriesRepository _employeesQueriesRepository;

    public CreateEmployeeService(IEmployeesCommandsRepository employeesCommandsRepository, IEmployeesQueriesRepository employeesQueriesRepository)
    {
        _employeesCommandsRepository = employeesCommandsRepository;
        _employeesQueriesRepository = employeesQueriesRepository;
    }

    public ResultType ResultType { get; private set; } = ResultType.None;

    public string? Message { get; private set; }

    public Dictionary<string, string[]> ValidationErrors { get; private set; } = new();

    public async Task CreateEmployeeAsync(string email, string name)
    {
        CreateEmployeeRequest request;
        try
        {
            request = CreateEmployeeRequest.Create(email, name);
        }
        catch (Exception ex)
        {
            ValidationErrors.Add("Request", new[] { ex.Message });
            ResultType = ResultType.Problems;
            return;
        }

        var existingEmployee = await _employeesQueriesRepository.GetAll().FirstOrDefaultAsync(x => x.Email == request.Email.Value);

        if (existingEmployee != null)
        {
            Message = "An employee exists.";
            ResultType = ResultType.Ok;
            return;
        }

        Employee employee;

        try
        {
            employee = EmployeeMapper.CreateEntity(request);
        }
        catch (Exception ex)
        {
            Message = $"Error creating employee entity: {ex.Message}";
            ResultType = ResultType.Error;
            return;
        }

        try
        {
            await _employeesCommandsRepository.AddAsync(employee);
        }
        catch (Exception)
        {
            Message = "Error saving employee to the database.";
            ResultType = ResultType.Error;
            return;
        }

        Message = "Employee created successfully.";
        ResultType = ResultType.Ok;
    }
}