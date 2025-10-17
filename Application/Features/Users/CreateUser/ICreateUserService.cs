using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.DTOs;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Users.CreateUser;

public interface ICreateUserService
{
    Task<bool> CreateUserAsync(CreateUserRequest request);
}
