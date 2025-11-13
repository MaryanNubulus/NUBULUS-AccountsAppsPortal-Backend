using Microsoft.EntityFrameworkCore;
using Nubulus.Backend.Infraestructure.Pgsql.Mappers;
using Nubulus.Backend.Infraestructure.Pgsql.Models;
using Nubulus.Domain.Abstractions;
using Nubulus.Domain.Entities.Account;
using Nubulus.Domain.ValueObjects;

namespace Nubulus.Backend.Infraestructure.Pgsql.Repositories;

public class AccountRepository : IAccountsRepository
{
    private readonly PostgreDBContext _dbContext;

    public AccountRepository(PostgreDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> AccountInfoExistsAsync(string name, string email, string phone, string numberId, CancellationToken cancellationToken = default, AccountId? excludeAccountId = null)
    {
        var nameExists = await _dbContext.Accounts.AnyAsync(a =>
            a.Name == name &&
            (excludeAccountId == null || a.Id != excludeAccountId.Value),
            cancellationToken);

        if (nameExists)
            return true;

        var emailInAccountExists = await _dbContext.Accounts.AnyAsync(a =>
            a.Email == email &&
            (excludeAccountId == null || a.Id != excludeAccountId.Value),
            cancellationToken);

        if (emailInAccountExists)
            return true;


        var phoneInAccountsExists = await _dbContext.Accounts.AnyAsync(a =>
            a.Phone == phone &&
            (excludeAccountId == null || a.Id != excludeAccountId.Value),
            cancellationToken);

        if (phoneInAccountsExists)
            return true;

        var numberIdExists = await _dbContext.Accounts.AnyAsync(a =>
            a.NumberId == numberId &&
            (excludeAccountId == null || a.Id != excludeAccountId.Value),
            cancellationToken);

        if (numberIdExists)
            return true;

        if (excludeAccountId != null)
        {
            var emailInUsersExists = await _dbContext.Users.AnyAsync(u =>
                u.Email == email &&
                !_dbContext.AccountUsers.Any(au =>
                    au.UserKey == u.Key &&
                    au.AccountKey == _dbContext.Accounts.First(a => a.Id == excludeAccountId.Value).Key &&
                    au.Creator == "Y"),
                cancellationToken);

            if (emailInUsersExists)
                return true;

            var phoneInUsersExists = await _dbContext.Users.AnyAsync(u =>
                u.Phone == phone &&
                !_dbContext.AccountUsers.Any(au =>
                    au.UserKey == u.Key &&
                    au.AccountKey == _dbContext.Accounts.First(a => a.Id == excludeAccountId.Value).Key &&
                    au.Creator == "Y"),
                cancellationToken);

            if (phoneInUsersExists)
                return true;
        }
        else
        {
            var emailInUsersExists = await _dbContext.Users.AnyAsync(u => u.Email == email, cancellationToken);

            if (emailInUsersExists)
                return true;

            var phoneInUsersExists = await _dbContext.Users.AnyAsync(u => u.Phone == phone, cancellationToken);
            if (phoneInUsersExists)
                return true;
        }

        return false;
    }

    public async Task<int> CountAccountsAsync(string? searchTerm, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Accounts.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(a =>
                a.Name.ToUpper().Contains(searchTerm.ToUpper()) ||
                a.Email.ToUpper().Contains(searchTerm.ToUpper()) ||
                a.Phone.ToUpper().Contains(searchTerm.ToUpper()) ||
                a.NumberId.ToUpper().Contains(searchTerm.ToUpper()));
        }

        return await query.CountAsync(cancellationToken);
    }

    public async Task<IQueryable<AccountEntity>> GetAccountsAsync(string? searchTerm, int? page, int? size, CancellationToken cancellationToken = default)
    {

        var joinquery = from a in _dbContext.Accounts
                        join au in _dbContext.AccountUsers on a.Key equals au.AccountKey
                        where au.Creator == "Y"
                        join u in _dbContext.Users on au.UserKey equals u.Key
                        select new { Account = a, AccountUser = au, User = u };

        var query = joinquery.OrderBy(a => a.Account.Id).AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(a =>
                a.Account.Name.ToUpper().Contains(searchTerm.ToUpper()) ||
                a.Account.Email.ToUpper().Contains(searchTerm.ToUpper()) ||
                a.Account.Phone.ToUpper().Contains(searchTerm.ToUpper()) ||
                a.Account.NumberId.ToUpper().Contains(searchTerm.ToUpper()) ||
                a.User.FullName.ToUpper().Contains(searchTerm.ToUpper()));
        }

        if (page.HasValue && size.HasValue)
        {
            query = query.Skip((page.Value - 1) * size.Value).Take(size.Value);
        }

        var results = await query.Select(a => new AccountEntity
        {
            AccountId = new AccountId(a.Account.Id),
            AccountKey = new AccountKey(a.Account.Key),
            Name = a.Account.Name,
            FullName = a.User.FullName,
            Email = new EmailAddress(a.Account.Email),
            Phone = a.Account.Phone,
            NumberId = a.Account.NumberId,
            Status = Status.Parse(a.Account.Status)
        }).ToListAsync(cancellationToken);

        return results.AsQueryable();
    }

    public async Task<AccountEntity> GetAccountByIdAsync(AccountId accountId, CancellationToken cancellationToken = default)
    {
        var joinquery = from a in _dbContext.Accounts
                        join au in _dbContext.AccountUsers on a.Key equals au.AccountKey
                        where au.Creator == "Y" && a.Id == accountId.Value
                        join u in _dbContext.Users on au.UserKey equals u.Key
                        select new { Account = a, AccountUser = au, User = u };

        var result = await joinquery.AsNoTracking()
            .Select(a => new AccountEntity
            {
                AccountId = new AccountId(a.Account.Id),
                AccountKey = new AccountKey(a.Account.Key),
                Name = a.Account.Name,
                FullName = a.User.FullName,
                Email = new EmailAddress(a.Account.Email),
                Phone = a.Account.Phone,
                Address = a.Account.Address,
                NumberId = a.Account.NumberId,
                Status = Status.Parse(a.Account.Status)
            })
            .FirstOrDefaultAsync(cancellationToken);

        return result!;
    }

    public async Task<AccountEntity> GetAccountByKeyAsync(AccountKey accountKey, CancellationToken cancellationToken = default)
    {
        var joinquery = from a in _dbContext.Accounts
                        join au in _dbContext.AccountUsers on a.Key equals au.AccountKey
                        where au.Creator == "Y" && a.Key == accountKey.Value
                        join u in _dbContext.Users on au.UserKey equals u.Key
                        select new { Account = a, AccountUser = au, User = u };

        var result = await joinquery.AsNoTracking()
            .Select(a => new AccountEntity
            {
                AccountId = new AccountId(a.Account.Id),
                AccountKey = new AccountKey(a.Account.Key),
                Name = a.Account.Name,
                FullName = a.User.FullName,
                Email = new EmailAddress(a.Account.Email),
                Phone = a.Account.Phone,
                Address = a.Account.Address,
                NumberId = a.Account.NumberId,
                Status = Status.Parse(a.Account.Status)
            })
            .FirstOrDefaultAsync(cancellationToken);

        return result!;
    }

    public async Task CreateAccountAsync(CreateAccount command, EmailAddress currentUserEmail, CancellationToken cancellationToken = default)
    {
        var account = new Account
        {
            Name = command.Name,
            Key = command.AccountKey.Value,
            Email = command.Email.Value,
            Phone = command.Phone,
            Address = command.Address,
            NumberId = command.NumberId,
            Status = "A"
        };
        var accountAuditRecord = account.ToAuditRecord(currentUserEmail.Value, RecordType.Create);


        var user = new User
        {
            Key = command.UserKey,
            ParentKey = account.Key,  // El compte on es crea és el parent
            FullName = command.FullName,
            Email = command.Email.Value,
            Phone = command.Phone,
        };
        var userAuditRecord = user.ToAuditRecord(currentUserEmail.Value, RecordType.Create);


        var accountUser = new AccountUser
        {
            Key = command.AccountUserKey,
            AccountKey = account.Key,
            UserKey = user.Key,
            Creator = "Y",
            Status = "A"
        };
        var accountUserAuditRecord = accountUser.ToAuditRecord(currentUserEmail.Value, RecordType.Create);


        await _dbContext.Accounts.AddAsync(account, cancellationToken);
        await _dbContext.AuditRecords.AddAsync(accountAuditRecord, cancellationToken);
        await _dbContext.Users.AddAsync(user, cancellationToken);
        await _dbContext.AuditRecords.AddAsync(userAuditRecord, cancellationToken);
        await _dbContext.AccountUsers.AddAsync(accountUser, cancellationToken);
        await _dbContext.AuditRecords.AddAsync(accountUserAuditRecord, cancellationToken);
    }

    public async Task PauseAccountAsync(AccountId accountId, EmailAddress currentUserEmail, CancellationToken cancellationToken = default)
    {
        var account = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == accountId.Value, cancellationToken);
        if (account == null)
            throw new InvalidOperationException("Account not found.");
        account.Status = "I";

        var accountUsers = await _dbContext.AccountUsers
            .Where(au => au.AccountKey == account.Key)
            .ToListAsync(cancellationToken);

        foreach (var accountUser in accountUsers)
        {
            accountUser.Status = "I";
            await _dbContext.AuditRecords.AddAsync(accountUser.ToAuditRecord(currentUserEmail.Value, RecordType.Pause), cancellationToken);

        }
        await _dbContext.AuditRecords.AddAsync(account.ToAuditRecord(currentUserEmail.Value, RecordType.Pause), cancellationToken);
    }

    public async Task ResumeAccountAsync(AccountId accountId, EmailAddress currentUserEmail, CancellationToken cancellationToken = default)
    {
        var account = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == accountId.Value, cancellationToken);
        if (account == null)
            throw new InvalidOperationException("Account not found.");
        account.Status = "A";
        var accountUsers = await _dbContext.AccountUsers
            .Where(au => au.AccountKey == account.Key)
            .ToListAsync(cancellationToken);
        foreach (var accountUser in accountUsers)
        {
            accountUser.Status = "A";
            await _dbContext.AuditRecords.AddAsync(accountUser.ToAuditRecord(currentUserEmail.Value, RecordType.Resume), cancellationToken);
        }
        await _dbContext.AuditRecords.AddAsync(account.ToAuditRecord(currentUserEmail.Value, RecordType.Resume), cancellationToken);
    }

    public async Task UpdateAccountAsync(UpdateAccount command, EmailAddress currentUserEmail, CancellationToken cancellationToken = default)
    {
        if (command == null)
            throw new ArgumentNullException(nameof(command));

        var account = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == command.AccountId.Value, cancellationToken);
        if (account == null)
            throw new InvalidOperationException("Account not found.");

        account.Name = command.Name;
        account.Phone = command.Phone;
        account.Address = command.Address;
        account.NumberId = command.NumberId;

        await _dbContext.AuditRecords.AddAsync(account.ToAuditRecord(currentUserEmail.Value, RecordType.Update), cancellationToken);
    }
}