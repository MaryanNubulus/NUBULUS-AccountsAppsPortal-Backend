using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.DTOs;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Abstractions;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Users.GetUsers;

public interface IGetUsersService
{
    Task<IGenericResponse<IEnumerable<UserInfoDTO>>> ExecuteAsync(string accountId);
}
