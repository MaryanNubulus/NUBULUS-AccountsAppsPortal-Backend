using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.DTOs;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Users.GetUsers;

public interface IGetUsersService
{
    Task<IEnumerable<UserInfoDTO>> GetUsersAsync();
}
