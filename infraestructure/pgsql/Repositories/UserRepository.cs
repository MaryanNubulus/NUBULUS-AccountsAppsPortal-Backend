using Microsoft.EntityFrameworkCore;
using Nubulus.Backend.Infraestructure.Pgsql.Mappers;
using Nubulus.Backend.Infraestructure.Pgsql.Models;
using Nubulus.Domain.Abstractions;
using Nubulus.Domain.Entities.User;
using Nubulus.Domain.ValueObjects;

namespace Nubulus.Backend.Infraestructure.Pgsql.Repositories;

public class UserRepository : IUsersRepository
{
    private readonly PostgreDBContext _dbContext;

    public UserRepository(PostgreDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> UserInfoExistsAsync(string name, string email, CancellationToken cancellationToken = default, UserId? excludeUserId = null)
    {
        var nameExists = await _dbContext.Users.AnyAsync(u =>
            u.Name == name &&
            (excludeUserId == null || u.Id != excludeUserId.Value),
            cancellationToken);

        if (nameExists)
            return true;

        var emailExists = await _dbContext.Users.AnyAsync(u =>
            u.Email == email &&
            (excludeUserId == null || u.Id != excludeUserId.Value),
            cancellationToken);

        return emailExists;
    }

    public async Task<bool> UserBelongsToAccountAsync(UserId userId, AccountId accountId, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId.Value, cancellationToken);
        if (user == null)
            return false;

        var account = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == accountId.Value, cancellationToken);
        if (account == null)
            return false;

        return await _dbContext.AccountUsers.AnyAsync(au =>
            au.UserKey == user.Key &&
            au.AccountKey == account.Key,
            cancellationToken);
    }

    public async Task<int> CountUsersAsync(AccountId accountId, string? searchTerm, CancellationToken cancellationToken = default)
    {
        var account = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == accountId.Value, cancellationToken);
        if (account == null)
            return 0;

        var query = from u in _dbContext.Users
                    join au in _dbContext.AccountUsers on u.Key equals au.UserKey
                    where au.AccountKey == account.Key
                    select u;

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(u =>
                u.Name.ToUpper().Contains(searchTerm.ToUpper()) ||
                u.Email.ToUpper().Contains(searchTerm.ToUpper()));
        }

        return await query.CountAsync(cancellationToken);
    }

    public async Task<IQueryable<UserEntity>> GetUsersAsync(AccountId accountId, string? searchTerm, int? page, int? size, CancellationToken cancellationToken = default)
    {
        var account = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == accountId.Value, cancellationToken);
        if (account == null)
            return Enumerable.Empty<UserEntity>().AsQueryable();

        var query = from u in _dbContext.Users
                    join au in _dbContext.AccountUsers on u.Key equals au.UserKey
                    where au.AccountKey == account.Key
                    orderby u.Id
                    select new { User = u, AccountUser = au };

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(x =>
                x.User.Name.ToUpper().Contains(searchTerm.ToUpper()) ||
                x.User.Email.ToUpper().Contains(searchTerm.ToUpper()));
        }

        if (page.HasValue && size.HasValue)
        {
            query = query.Skip((page.Value - 1) * size.Value).Take(size.Value);
        }

        var results = await query
            .AsNoTracking()
            .Select(x => new UserEntity
            {
                UserId = new UserId(x.User.Id),
                UserKey = new UserKey(x.User.Key),
                Name = x.User.Name,
                Email = new EmailAddress(x.User.Email),
                Status = Status.Parse(x.AccountUser.Status),
                IsCreator = x.AccountUser.Creator == "Y"
            })
            .ToListAsync(cancellationToken);

        return results.AsQueryable();
    }

    public async Task<UserEntity> GetUserByIdAsync(UserId userId, AccountId accountId, CancellationToken cancellationToken = default)
    {
        var account = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == accountId.Value, cancellationToken);
        if (account == null)
            return null!;

        var result = await (from u in _dbContext.Users
                            join au in _dbContext.AccountUsers on u.Key equals au.UserKey
                            where u.Id == userId.Value && au.AccountKey == account.Key
                            select new { User = u, AccountUser = au })
            .AsNoTracking()
            .Select(x => new UserEntity
            {
                UserId = new UserId(x.User.Id),
                UserKey = new UserKey(x.User.Key),
                Name = x.User.Name,
                Email = new EmailAddress(x.User.Email),
                Status = Status.Parse(x.AccountUser.Status),
                IsCreator = x.AccountUser.Creator == "Y"
            })
            .FirstOrDefaultAsync(cancellationToken);

        return result!;
    }

    public async Task<UserEntity> GetUserByKeyAsync(UserKey userKey, CancellationToken cancellationToken = default)
    {
        var result = await _dbContext.Users
            .AsNoTracking()
            .Where(u => u.Key == userKey.Value)
            .Select(u => new UserEntity
            {
                UserId = new UserId(u.Id),
                UserKey = new UserKey(u.Key),
                Name = u.Name,
                Email = new EmailAddress(u.Email),
                Status = Status.Active
            })
            .FirstOrDefaultAsync(cancellationToken);

        return result!;
    }

    public async Task CreateUserAsync(CreateUser command, AccountId accountId, EmailAddress currentUserEmail, CancellationToken cancellationToken = default)
    {
        // Verificar que el Account existe
        var account = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == accountId.Value, cancellationToken);
        if (account == null)
            throw new InvalidOperationException("Account not found.");

        var user = new User
        {
            Key = command.UserKey.Value,
            Name = command.Name,
            Email = command.Email.Value,
        };

        var userAuditRecord = user.ToAuditRecord(currentUserEmail.Value, RecordType.Create);

        // Crear la relación AccountUser
        var accountUserKey = Guid.NewGuid().ToString();
        var accountUser = new AccountUser
        {
            Key = accountUserKey,
            AccountKey = account.Key,
            UserKey = user.Key,
            Creator = "N",  // No es creador del account
            Status = "A"
        };

        var accountUserAuditRecord = accountUser.ToAuditRecord(currentUserEmail.Value, RecordType.Create);

        await _dbContext.Users.AddAsync(user, cancellationToken);
        await _dbContext.AuditRecords.AddAsync(userAuditRecord, cancellationToken);
        await _dbContext.AccountUsers.AddAsync(accountUser, cancellationToken);
        await _dbContext.AuditRecords.AddAsync(accountUserAuditRecord, cancellationToken);
    }

    public async Task UpdateUserAsync(UpdateUser command, EmailAddress currentUserEmail, CancellationToken cancellationToken = default)
    {
        if (command == null)
            throw new ArgumentNullException(nameof(command));

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Key == command.UserKey.Value, cancellationToken);
        if (user == null)
            throw new InvalidOperationException("User not found.");

        user.Name = command.Name;
        user.Email = command.Email.Value;

        await _dbContext.AuditRecords.AddAsync(user.ToAuditRecord(currentUserEmail.Value, RecordType.Update), cancellationToken);
    }

    public async Task PauseUserAsync(UserId userId, AccountId accountId, EmailAddress currentUserEmail, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId.Value, cancellationToken);
        if (user == null)
            throw new InvalidOperationException("User not found.");

        var account = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == accountId.Value, cancellationToken);
        if (account == null)
            throw new InvalidOperationException("Account not found.");

        // Pausar la relación AccountUser, no el User directamente
        var accountUser = await _dbContext.AccountUsers.FirstOrDefaultAsync(au =>
            au.UserKey == user.Key &&
            au.AccountKey == account.Key,
            cancellationToken);

        if (accountUser == null)
            throw new InvalidOperationException("User does not belong to this account.");

        accountUser.Status = "I";
        await _dbContext.AuditRecords.AddAsync(accountUser.ToAuditRecord(currentUserEmail.Value, RecordType.Pause), cancellationToken);
    }

    public async Task ResumeUserAsync(UserId userId, AccountId accountId, EmailAddress currentUserEmail, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId.Value, cancellationToken);
        if (user == null)
            throw new InvalidOperationException("User not found.");

        var account = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == accountId.Value, cancellationToken);
        if (account == null)
            throw new InvalidOperationException("Account not found.");

        // Reactivar la relación AccountUser
        var accountUser = await _dbContext.AccountUsers.FirstOrDefaultAsync(au =>
            au.UserKey == user.Key &&
            au.AccountKey == account.Key,
            cancellationToken);

        if (accountUser == null)
            throw new InvalidOperationException("User does not belong to this account.");

        accountUser.Status = "A";
        await _dbContext.AuditRecords.AddAsync(accountUser.ToAuditRecord(currentUserEmail.Value, RecordType.Resume), cancellationToken);
    }
}
