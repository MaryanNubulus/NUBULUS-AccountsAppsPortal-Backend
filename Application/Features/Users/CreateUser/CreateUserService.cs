using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Abstractions;

using NUBULUS.AccountsAppsPortalBackEnd.Domain.Entities;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Users.CreateUser;

public class CreateUserService : ICreateUserService
{
    private readonly IUsersCommandsRepository _usersCommandsRepository;

    public CreateUserService(IUsersCommandsRepository usersCommandsRepository)
    {
        _usersCommandsRepository = usersCommandsRepository;
    }

    public async Task<bool> CreateUserAsync(string email, string name)
    {
        var user = User.Create(email, name);        

        return await _usersCommandsRepository.AddAsync(user);        
    }
}   