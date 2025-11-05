using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.DTOs;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Abstractions;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Users.ActivateDeactivateUser;

public interface IActivateDeactivateUserService
{
    Task<IGenericResponse<UserInfoDTO>> ExecuteAsync(string accountId, string userId, bool activate);
}
