namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Users.ExistUser;

public interface IExistUserService
{
    Task<bool> ExistUserAsync(string email);
}
