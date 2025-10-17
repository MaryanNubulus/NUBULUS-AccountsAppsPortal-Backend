

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.DTOs;

public class GetUsersResponse
{
    public bool Success { get; set; } = false;
    public string? Message { get; set; } = null;
    public List<UserInfoDTO>? Users { get; set; } = null;
}