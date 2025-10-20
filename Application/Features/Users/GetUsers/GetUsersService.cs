using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Abstractions;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.DTOs;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Mappers;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Users.GetUsers;

public class GetUsersService : IGetUsersService
{
    private readonly IUsersQueriesRepository _usersQueriesRepository;

    public GetUsersService(IUsersQueriesRepository usersQueriesRepository)
    {
        _usersQueriesRepository = usersQueriesRepository;
    }

    public async Task<IEnumerable<UserInfoDTO>> GetUsersAsync()
    {
        return await Task.FromResult(_usersQueriesRepository.GetAll().Select(UserMapper.ToDTO).ToList());

    }
}