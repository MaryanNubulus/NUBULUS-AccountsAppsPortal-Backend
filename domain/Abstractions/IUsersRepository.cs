using Nubulus.Domain.Entities.User;
using Nubulus.Domain.ValueObjects;

namespace Nubulus.Domain.Abstractions;

public interface IUsersRepository
{
    Task<UserEntity> GetUserByKeyAsync(UserKey userKey, CancellationToken cancellationToken = default);
    Task<UserEntity> GetUserByIdAsync(UserId userId, AccountId accountId, CancellationToken cancellationToken = default);
    Task<int> CountUsersAsync(AccountId accountId, string? searchTerm, CancellationToken cancellationToken = default);
    Task<IQueryable<UserEntity>> GetUsersAsync(AccountId accountId, string? searchTerm, int? page, int? size, CancellationToken cancellationToken = default);
    Task<bool> UserInfoExistsAsync(string name, string email, CancellationToken cancellationToken = default, UserId? excludeUserId = null);
    Task<bool> UserBelongsToAccountAsync(UserId userId, AccountId accountId, CancellationToken cancellationToken = default);
    Task CreateUserAsync(CreateUser command, AccountId accountId, EmailAddress currentUserEmail, CancellationToken cancellationToken = default);
    Task UpdateUserAsync(UpdateUser command, EmailAddress currentUserEmail, CancellationToken cancellationToken = default);
    Task PauseUserAsync(UserId userId, AccountId accountId, EmailAddress currentUserEmail, CancellationToken cancellationToken = default);
    Task ResumeUserAsync(UserId userId, AccountId accountId, EmailAddress currentUserEmail, CancellationToken cancellationToken = default);

    // Mètodes per compartir usuaris
    Task<int> CountUsersToShareAsync(AccountId accountId, string? searchTerm, CancellationToken cancellationToken = default);
    Task<IQueryable<UserEntity>> GetUsersToShareAsync(AccountId accountId, string? searchTerm, int? page, int? size, CancellationToken cancellationToken = default);
    Task<int> CountSharedUsersAsync(AccountId accountId, string? searchTerm, CancellationToken cancellationToken = default);
    Task<IQueryable<UserEntity>> GetSharedUsersAsync(AccountId accountId, string? searchTerm, int? page, int? size, CancellationToken cancellationToken = default);
    Task ShareUserAsync(UserId userId, AccountId accountId, EmailAddress currentUserEmail, CancellationToken cancellationToken = default);
    Task UnshareUserAsync(UserId userId, AccountId accountId, EmailAddress currentUserEmail, CancellationToken cancellationToken = default);
}
