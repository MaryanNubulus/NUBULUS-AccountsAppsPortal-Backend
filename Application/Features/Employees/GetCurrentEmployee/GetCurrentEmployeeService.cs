using MongoDB.Driver.Linq;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Enums;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.DTOs;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.ValueObjects;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Abstractions;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Mappers;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Employees.GetCurrentEmployee;

public class GetCurrentEmployeeService : IGetCurrentEmployeeService
{
    private readonly IEmployeesQueriesRepository _employeesQueriesRepository;

    public GetCurrentEmployeeService(IEmployeesQueriesRepository employeesQueriesRepository)
    {
        _employeesQueriesRepository = employeesQueriesRepository;
    }

    public ResultType ResultType { get; private set; } = ResultType.None;

    public string? Message { get; private set; }

    public Dictionary<string, string[]> ValidationErrors { get; private set; } = new();
    public async Task<EmployeeInfoDTO?> GetCurrentEmployeeAsync(string email)
    {
        Email emailObject;

        try
        {
            emailObject = Email.Create(email);
        }
        catch (Exception ex)
        {
            ValidationErrors.Add("Email", new[] { ex.Message });
            ResultType = ResultType.Problems;
            return null;
        }

        var employee = await _employeesQueriesRepository.GetAll().FirstOrDefaultAsync(x => x.Email == emailObject.Value);
        if (employee == null)
        {
            ResultType = ResultType.Ok;
            Message = "Employee not found.";
            return null;
        }

        ResultType = ResultType.Ok;
        Message = "Employee found.";

        return EmployeeMapper.ToDTO(employee);

    }
}
