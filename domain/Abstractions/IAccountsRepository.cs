using Nubulus.Domain.Entities.Account;

namespace Nubulus.Domain.Abstractions;

public interface IAccountsRepository
{
    Task<AccountEntity> GetAccountByKeyAsync(string accountKey, CancellationToken cancellationToken = default);
    Task<AccountEntity> GetAccountByIdAsync(int accountId, CancellationToken cancellationToken = default);
    Task<IQueryable<AccountEntity>> GetAccountsAsync(string? searchTerm, CancellationToken cancellationToken = default);
    Task CreateAccountAsync(CreateAccount command, CancellationToken cancellationToken = default);
    Task UpdateAccountAsync(AccountEntity command, CancellationToken cancellationToken = default);
    Task PauseAccountAsync(int accountId, CancellationToken cancellationToken = default);
    Task ResumeAccountAsync(int accountId, CancellationToken cancellationToken = default);
    Task<bool> AccountInfoExistsAsync(string name, string email, string phone, string numberId, CancellationToken cancellationToken = default);
}
