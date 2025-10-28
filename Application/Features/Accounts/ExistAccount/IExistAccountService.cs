using MongoDB.Driver.Linq;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Requests;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Abstractions;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Accounts.ExistAccount;

public interface IExistAccountService
{
    Task<bool> ExistAccountAsync(CreateAccountRequest request);
}

public class ExistAccountService : IExistAccountService
{
    private readonly IAccountsQueriesRepository _accountsQueriesRepository;
    private readonly IUsersQueriesRepository _usersQueriesRepository;

    public ExistAccountService(IAccountsQueriesRepository accountsQueriesRepository, IUsersQueriesRepository usersQueriesRepository)
    {
        _accountsQueriesRepository = accountsQueriesRepository;
        _usersQueriesRepository = usersQueriesRepository;
    }

    public async Task<bool> ExistAccountAsync(CreateAccountRequest request)
    {
        var accountNameExists = await _accountsQueriesRepository.GetAll().AnyAsync(a => a.Name == request.AccountName);
        if (accountNameExists)
        {
            return true;
        }

        return false;
    }
}