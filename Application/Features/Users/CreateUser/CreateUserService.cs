using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Abstractions;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.DTOs;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Mappers;
using NUBULUS.AccountsAppsPortalBackEnd.Domain.Entities;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Users.CreateUser;

public class CreateUserService : ICreateUserService
{
    private readonly IUsersCommandsRepository _usersCommandsRepository;

    public CreateUserService(IUsersCommandsRepository usersCommandsRepository)
    {
        _usersCommandsRepository = usersCommandsRepository;
    }

    public async Task<bool> CreateUserAsync(CreateUserRequest request)
    {
        var user = UserMapper.CreateEntity(request);

        return await _usersCommandsRepository.AddAsync(user);
    }
}