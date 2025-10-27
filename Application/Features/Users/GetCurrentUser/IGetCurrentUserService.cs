using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.DTOs;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Users.GetCurrentUser;

public interface IGetCurrentUserService
{
    Task<UserInfoDTO?> GetCurrentUserAsync(string email);
}
