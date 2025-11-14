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

    public async Task<bool> UserInfoExistsAsync(string name, string email, string phone, CancellationToken cancellationToken = default, UserId? excludeUserId = null)
    {
        var nameExists = await _dbContext.Users.AnyAsync(u =>
            u.FullName == name &&
            (excludeUserId == null || u.Id != excludeUserId.Value),
            cancellationToken);

        if (nameExists)
            return true;

        var emailExists = await _dbContext.Users.AnyAsync(u =>
            u.Email == email &&
            (excludeUserId == null || u.Id != excludeUserId.Value),
            cancellationToken);

        if (emailExists)
            return true;
        var phoneExists = await _dbContext.Users.AnyAsync(u =>
            u.Phone == phone &&
            (excludeUserId == null || u.Id != excludeUserId.Value),
            cancellationToken);

        return phoneExists;
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
                    where au.AccountKey == account.Key && u.ParentKey == account.Key
                    select u;

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(u =>
                u.FullName.ToUpper().Contains(searchTerm.ToUpper()) ||
                u.Email.ToUpper().Contains(searchTerm.ToUpper()) ||
                u.Phone.Contains(searchTerm));
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
                    where au.AccountKey == account.Key && u.ParentKey == account.Key
                    orderby u.Id
                    select new { User = u, AccountUser = au };

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(x =>
                x.User.FullName.ToUpper().Contains(searchTerm.ToUpper()) ||
                x.User.Email.ToUpper().Contains(searchTerm.ToUpper()) ||
                x.User.Phone.Contains(searchTerm));
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
                ParentKey = new AccountKey(x.User.ParentKey),
                FullName = x.User.FullName,
                Email = new EmailAddress(x.User.Email),
                Phone = x.User.Phone,
                Password = x.User.Password,
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
                ParentKey = new AccountKey(x.User.ParentKey),
                FullName = x.User.FullName,
                Email = new EmailAddress(x.User.Email),
                Phone = x.User.Phone,
                Password = x.User.Password,
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
                ParentKey = new AccountKey(u.ParentKey),
                FullName = u.FullName,
                Email = new EmailAddress(u.Email),
                Phone = u.Phone,
                Password = u.Password,
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
            ParentKey = command.ParentKey.Value,
            FullName = command.FullName,
            Email = command.Email.Value,
            Phone = command.Phone,
            Password = command.Password ?? string.Empty,
            Status = "A"
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

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == command.UserId.Value, cancellationToken);
        if (user == null)
            throw new InvalidOperationException("User not found.");

        user.FullName = command.FullName;
        user.Email = command.Email.Value;
        user.Phone = command.Phone;

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

        // Cambiar estado del usuario a "I" (Inactivo)
        user.Status = "I";
        await _dbContext.AuditRecords.AddAsync(user.ToAuditRecord(currentUserEmail.Value, RecordType.Pause), cancellationToken);

        // Pausar todas las relaciones AccountUser del usuario
        var accountUsers = await _dbContext.AccountUsers
            .Where(au => au.UserKey == user.Key)
            .ToListAsync(cancellationToken);

        foreach (var accountUser in accountUsers)
        {
            accountUser.Status = "I";
            await _dbContext.AuditRecords.AddAsync(accountUser.ToAuditRecord(currentUserEmail.Value, RecordType.Pause), cancellationToken);
        }
    }

    public async Task ResumeUserAsync(UserId userId, AccountId accountId, EmailAddress currentUserEmail, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId.Value, cancellationToken);
        if (user == null)
            throw new InvalidOperationException("User not found.");

        var account = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == accountId.Value, cancellationToken);
        if (account == null)
            throw new InvalidOperationException("Account not found.");

        // Cambiar estado del usuario a "A" (Activo)
        user.Status = "A";
        await _dbContext.AuditRecords.AddAsync(user.ToAuditRecord(currentUserEmail.Value, RecordType.Resume), cancellationToken);

        // Reactivar todas las relaciones AccountUser del usuario
        var accountUsers = await _dbContext.AccountUsers
            .Where(au => au.UserKey == user.Key)
            .ToListAsync(cancellationToken);

        foreach (var accountUser in accountUsers)
        {
            accountUser.Status = "A";
            await _dbContext.AuditRecords.AddAsync(accountUser.ToAuditRecord(currentUserEmail.Value, RecordType.Resume), cancellationToken);
        }
    }

    // Mètodes per compartir usuaris

    public async Task<int> CountUsersToShareAsync(AccountId accountId, string? searchTerm, CancellationToken cancellationToken = default)
    {
        var account = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == accountId.Value, cancellationToken);
        if (account == null)
            return 0;

        // Obtenir tots els usuaris excepte els que ja tenen relació amb aquest compte
        var usersAlreadyInAccount = _dbContext.AccountUsers
            .Where(au => au.AccountKey == account.Key)
            .Select(au => au.UserKey);

        var query = _dbContext.Users
            .Where(u => !usersAlreadyInAccount.Contains(u.Key));

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(u =>
                u.FullName.ToUpper().Contains(searchTerm.ToUpper()) ||
                u.Email.ToUpper().Contains(searchTerm.ToUpper()) ||
                u.Phone.Contains(searchTerm));
        }

        return await query.CountAsync(cancellationToken);
    }

    public async Task<IQueryable<UserEntity>> GetUsersToShareAsync(AccountId accountId, string? searchTerm, int? page, int? size, CancellationToken cancellationToken = default)
    {
        var account = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == accountId.Value, cancellationToken);
        if (account == null)
            return Enumerable.Empty<UserEntity>().AsQueryable();

        // Obtenir tots els usuaris excepte els que ja tenen relació amb aquest compte
        var usersAlreadyInAccount = _dbContext.AccountUsers
            .Where(au => au.AccountKey == account.Key)
            .Select(au => au.UserKey)
            .ToList();

        IQueryable<User> query = _dbContext.Users
            .Where(u => !usersAlreadyInAccount.Contains(u.Key))
            .OrderBy(u => u.Id);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(u =>
                u.FullName.ToUpper().Contains(searchTerm.ToUpper()) ||
                u.Email.ToUpper().Contains(searchTerm.ToUpper()) ||
                u.Phone.Contains(searchTerm));
        }

        if (page.HasValue && size.HasValue)
        {
            query = query.Skip((page.Value - 1) * size.Value).Take(size.Value);
        }

        var results = await query
            .AsNoTracking()
            .Select(u => new UserEntity
            {
                UserId = new UserId(u.Id),
                UserKey = new UserKey(u.Key),
                ParentKey = new AccountKey(u.ParentKey),
                FullName = u.FullName,
                Email = new EmailAddress(u.Email),
                Phone = u.Phone,
                Password = u.Password,
                Status = Status.Parse(u.Status),
                IsCreator = false
            })
            .ToListAsync(cancellationToken);

        return results.AsQueryable();
    }

    public async Task<int> CountSharedUsersAsync(AccountId accountId, string? searchTerm, CancellationToken cancellationToken = default)
    {
        var account = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == accountId.Value, cancellationToken);
        if (account == null)
            return 0;

        // Retornar només usuaris compartits (ParentKey diferent del compte actual)
        var query = from u in _dbContext.Users
                    join au in _dbContext.AccountUsers on u.Key equals au.UserKey
                    where au.AccountKey == account.Key && u.ParentKey != account.Key
                    select u;

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(u =>
                u.FullName.ToUpper().Contains(searchTerm.ToUpper()) ||
                u.Email.ToUpper().Contains(searchTerm.ToUpper()) ||
                u.Phone.Contains(searchTerm));
        }

        return await query.CountAsync(cancellationToken);
    }

    public async Task<IQueryable<UserEntity>> GetSharedUsersAsync(AccountId accountId, string? searchTerm, int? page, int? size, CancellationToken cancellationToken = default)
    {
        var account = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == accountId.Value, cancellationToken);
        if (account == null)
            return Enumerable.Empty<UserEntity>().AsQueryable();

        // Retornar només usuaris compartits (ParentKey diferent del compte actual)
        var query = from u in _dbContext.Users
                    join au in _dbContext.AccountUsers on u.Key equals au.UserKey
                    where au.AccountKey == account.Key && u.ParentKey != account.Key
                    orderby u.Id
                    select new { User = u, AccountUser = au };

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(x =>
                x.User.FullName.ToUpper().Contains(searchTerm.ToUpper()) ||
                x.User.Email.ToUpper().Contains(searchTerm.ToUpper()) ||
                x.User.Phone.Contains(searchTerm));
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
                ParentKey = new AccountKey(x.User.ParentKey),
                FullName = x.User.FullName,
                Email = new EmailAddress(x.User.Email),
                Phone = x.User.Phone,
                Password = x.User.Password,
                Status = Status.Parse(x.AccountUser.Status),
                IsCreator = x.AccountUser.Creator == "Y"
            })
            .ToListAsync(cancellationToken);

        return results.AsQueryable();
    }

    public async Task ShareUserAsync(UserId userId, AccountId accountId, EmailAddress currentUserEmail, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId.Value, cancellationToken);
        if (user == null)
            throw new InvalidOperationException("User not found.");

        var account = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == accountId.Value, cancellationToken);
        if (account == null)
            throw new InvalidOperationException("Account not found.");

        // Verificar que no existeixi ja la relació
        var existingRelation = await _dbContext.AccountUsers
            .AnyAsync(au => au.UserKey == user.Key && au.AccountKey == account.Key, cancellationToken);

        if (existingRelation)
            throw new InvalidOperationException("User is already shared with this account.");

        // Crear la relació AccountUser
        var accountUserKey = Guid.NewGuid().ToString();
        var accountUser = new AccountUser
        {
            Key = accountUserKey,
            AccountKey = account.Key,
            UserKey = user.Key,
            Creator = "N",  // No és creador quan es comparteix
            Status = "A"
        };

        var accountUserAuditRecord = accountUser.ToAuditRecord(currentUserEmail.Value, RecordType.Create);

        await _dbContext.AccountUsers.AddAsync(accountUser, cancellationToken);
        await _dbContext.AuditRecords.AddAsync(accountUserAuditRecord, cancellationToken);
    }

    public async Task UnshareUserAsync(UserId userId, AccountId accountId, EmailAddress currentUserEmail, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId.Value, cancellationToken);
        if (user == null)
            throw new InvalidOperationException("User not found.");

        var account = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == accountId.Value, cancellationToken);
        if (account == null)
            throw new InvalidOperationException("Account not found.");

        // Verificar que l'usuari no sigui el creador del compte
        var accountUser = await _dbContext.AccountUsers
            .FirstOrDefaultAsync(au => au.UserKey == user.Key && au.AccountKey == account.Key, cancellationToken);

        if (accountUser == null)
            throw new InvalidOperationException("User is not shared with this account.");

        if (accountUser.Creator == "Y")
            throw new InvalidOperationException("Cannot unshare the creator of the account.");

        // Eliminar la relació
        var accountUserAuditRecord = accountUser.ToAuditRecord(currentUserEmail.Value, RecordType.Delete);

        _dbContext.AccountUsers.Remove(accountUser);
        await _dbContext.AuditRecords.AddAsync(accountUserAuditRecord, cancellationToken);
    }
}
