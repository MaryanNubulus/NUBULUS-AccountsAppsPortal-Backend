using MongoDB.Driver.Linq;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Abstractions;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.DTOs;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Mappers;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Users.GetCurrentUser;

public class GetCurrentUserService : IGetCurrentUserService
{
    private readonly IUsersQueriesRepository _usersQueriesRepository;

    public GetCurrentUserService(IUsersQueriesRepository usersQueriesRepository)
    {
        _usersQueriesRepository = usersQueriesRepository;
    }

    public async Task<UserInfoDTO?> GetCurrentUserAsync(string email)
    {
        var user = await _usersQueriesRepository.GetAll().FirstOrDefaultAsync(x => x.Email == email);
        if (user == null) return null;
        return UserMapper.ToDTO(user);
    }
}
