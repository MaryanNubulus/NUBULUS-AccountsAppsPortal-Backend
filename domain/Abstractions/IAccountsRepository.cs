using Nubulus.Domain.Entities.Account;
using Nubulus.Domain.ValueObjects;

namespace Nubulus.Domain.Abstractions;

public interface IAccountsRepository
{
    Task<AccountEntity> GetAccountByKeyAsync(AccountKey accountKey, CancellationToken cancellationToken = default);
    Task<AccountEntity> GetAccountByIdAsync(AccountId accountId, CancellationToken cancellationToken = default);
    Task<int> CountAccountsAsync(string? searchTerm, CancellationToken cancellationToken = default);
    Task<IQueryable<AccountEntity>> GetAccountsAsync(string? searchTerm, int? page, int? size, CancellationToken cancellationToken = default);
    Task<bool> AccountInfoExistsAsync(string name, string email, string phone, string numberId, CancellationToken cancellationToken = default, AccountId? excludeAccountId = null);
    Task<AccountId> CreateAccountAsync(CreateAccount command, EmailAddress currentUserEmail, CancellationToken cancellationToken = default);
    Task UpdateAccountAsync(UpdateAccount command, EmailAddress currentUserEmail, CancellationToken cancellationToken = default);
    Task PauseAccountAsync(AccountId accountId, EmailAddress currentUserEmail, CancellationToken cancellationToken = default);
    Task ResumeAccountAsync(AccountId accountId, EmailAddress currentUserEmail, CancellationToken cancellationToken = default);
}
