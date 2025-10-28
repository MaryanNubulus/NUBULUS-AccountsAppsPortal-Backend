using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.DTOs;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Responses;

public record GetEmployeesResponse(List<EmployeeInfoDTO> Employees);
