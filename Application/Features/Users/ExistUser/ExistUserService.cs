using MongoDB.Driver.Linq;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Abstractions;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Users.ExistUser;

public class ExistUserService : IExistUserService
{
    private readonly IUsersQueriesRepository _usersQueriesRepository;

    public ExistUserService(IUsersQueriesRepository usersQueriesRepository)
    {
        _usersQueriesRepository = usersQueriesRepository;
    }

    public async Task<bool> ExistUserAsync(string email)
    {
        return await _usersQueriesRepository.GetAll().AnyAsync(x => x.Email == email);
    }

}
