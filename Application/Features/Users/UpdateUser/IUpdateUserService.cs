using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.DTOs;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Requests;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Abstractions;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Users.UpdateUser;

public interface IUpdateUserService
{
    Task<IGenericResponse<UserInfoDTO>> ExecuteAsync(string accountId, string userId, UpdateUserRequest request);
}
