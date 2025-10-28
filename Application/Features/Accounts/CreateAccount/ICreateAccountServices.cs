using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Requests;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Abstractions;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Accounts.CreateAccount;

public interface ICreateAccountServices
{
    Task CreateAccountAsync(CreateAccountRequest request);
}

public class CreateAccountServices : ICreateAccountServices
{
    private readonly IAccountsCommandsRepository _accountsCommandsRepository;
    private readonly IUsersCommandsRepository _usersCommandsRepository;

    public CreateAccountServices(IAccountsCommandsRepository accountsCommandsRepository, IUsersCommandsRepository usersCommandsRepository)
    {
        _accountsCommandsRepository = accountsCommandsRepository;
        _usersCommandsRepository = usersCommandsRepository;
    }

    public async Task CreateAccountAsync(CreateAccountRequest request)
    {
        // Implementation for creating an account
        await Task.CompletedTask;
    }
}