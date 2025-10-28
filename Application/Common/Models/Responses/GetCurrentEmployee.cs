using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.DTOs;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Responses;

public class GetCurrentEmployeeResponse
{
    public EmployeeInfoDTO? Employee { get; set; } = null;
}