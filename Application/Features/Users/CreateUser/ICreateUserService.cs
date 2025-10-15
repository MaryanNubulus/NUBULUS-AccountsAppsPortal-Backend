namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Users.CreateUser;

public interface ICreateUserService
{
    Task<bool> CreateUserAsync(string email, string name);
}
