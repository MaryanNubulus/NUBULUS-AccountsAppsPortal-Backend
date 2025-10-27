

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.DTOs;

public class GetEmployeesResponse
{
    public bool Success { get; set; } = false;
    public string? Message { get; set; } = null;
    public List<EmployeeInfoDTO>? Employees { get; set; } = null;
}