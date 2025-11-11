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
    private readonly AuditRecordRepository _auditRecordRepository;
    public AccountRepository(PostgreDBContext dbContext, AuditRecordRepository auditRecordRepository)
    {
        _dbContext = dbContext;
        _auditRecordRepository = auditRecordRepository;
    }

    public async Task<bool> AccountInfoExistsAsync(string name, string email, string phone, string numberId, CancellationToken cancellationToken = default, int? excludeAccountId = null)
    {
        var accountExists = await _dbContext.Accounts.AnyAsync(a =>
            (a.Name == name ||
             a.Email == email ||
             a.Phone == phone ||
             a.NumberId == numberId) &&
            (!excludeAccountId.HasValue || a.Id != excludeAccountId.Value),
            cancellationToken);

        if (accountExists)
        {
            return true;
        }
        if (excludeAccountId != null)
        {
            var account = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == excludeAccountId.Value, cancellationToken);
            if (account != null && account.Email == email)
            {
                return false;
            }
        }
        var userExists = await _dbContext.Users.AnyAsync(u =>
            u.Email == email, cancellationToken);

        return userExists;
    }

    public async Task CreateAccountAsync(CreateAccount command, EmailAddress currentUserEmail, CancellationToken cancellationToken = default)
    {
        var account = new Account
        {
            Name = command.Name,
            Key = command.AccountKey,
            Email = command.Email.Value,
            Phone = command.Phone,
            Address = command.Address,
            NumberId = command.NumberId,
            Status = "A"
        };
        var accountAuditRecord = account.ToAuditRecord(currentUserEmail.Value, RecordType.Create);
        await _auditRecordRepository.CreateAuditRecordAsync(accountAuditRecord, cancellationToken);

        var user = new User
        {
            Key = command.UserKey,
            Name = command.FullName,
            Email = command.Email.Value,
        };
        var userAuditRecord = user.ToAuditRecord(currentUserEmail.Value, RecordType.Create);
        await _auditRecordRepository.CreateAuditRecordAsync(userAuditRecord, cancellationToken);

        var accountUser = new AccountUser
        {
            Key = command.AccountUserKey,
            AccountKey = account.Key,
            UserKey = user.Key,
            Creator = "Y"
        };
        var accountUserAuditRecord = accountUser.ToAuditRecord(currentUserEmail.Value, RecordType.Create);
        await _auditRecordRepository.CreateAuditRecordAsync(accountUserAuditRecord, cancellationToken);

        await _dbContext.Accounts.AddAsync(account, cancellationToken);
        await _dbContext.Users.AddAsync(user, cancellationToken);
        await _dbContext.AccountUsers.AddAsync(accountUser, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
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
                a.User.Name.ToUpper().Contains(searchTerm.ToUpper()));
        }

        if (page.HasValue && size.HasValue)
        {
            query = query.Skip((page.Value - 1) * size.Value).Take(size.Value);
        }

        var results = await query.Select(a => new AccountEntity
        {
            Id = a.Account.Id,
            AccountKey = new AccountKey(a.Account.Key),
            Name = a.Account.Name,
            FullName = a.User.Name,
            Email = new EmailAddress(a.Account.Email),
            Phone = a.Account.Phone,
            NumberId = a.Account.NumberId,
            Status = AccountStatus.Parse(a.Account.Status)
        }).ToListAsync(cancellationToken);

        return results.AsQueryable();
    }

    public async Task<AccountEntity> GetAccountByIdAsync(int accountId, CancellationToken cancellationToken = default)
    {
        var joinquery = from a in _dbContext.Accounts
                        join au in _dbContext.AccountUsers on a.Key equals au.AccountKey
                        where au.Creator == "Y" && a.Id == accountId
                        join u in _dbContext.Users on au.UserKey equals u.Key
                        select new { Account = a, AccountUser = au, User = u };

        var result = await joinquery.AsNoTracking()
            .Select(a => new AccountEntity
            {
                Id = a.Account.Id,
                AccountKey = new AccountKey(a.Account.Key),
                Name = a.Account.Name,
                FullName = a.User.Name,
                Email = new EmailAddress(a.Account.Email),
                Phone = a.Account.Phone,
                Address = a.Account.Address,
                NumberId = a.Account.NumberId,
                Status = AccountStatus.Parse(a.Account.Status)
            })
            .FirstOrDefaultAsync(cancellationToken);

        return result!;
    }

    public async Task<AccountEntity> GetAccountByKeyAsync(string accountKey, CancellationToken cancellationToken = default)
    {
        var joinquery = from a in _dbContext.Accounts
                        join au in _dbContext.AccountUsers on a.Key equals au.AccountKey
                        where au.Creator == "Y" && a.Key == accountKey
                        join u in _dbContext.Users on au.UserKey equals u.Key
                        select new { Account = a, AccountUser = au, User = u };

        var result = await joinquery.AsNoTracking()
            .Select(a => new AccountEntity
            {
                Id = a.Account.Id,
                AccountKey = new AccountKey(a.Account.Key),
                Name = a.Account.Name,
                FullName = a.User.Name,
                Email = new EmailAddress(a.Account.Email),
                Phone = a.Account.Phone,
                Address = a.Account.Address,
                NumberId = a.Account.NumberId,
                Status = AccountStatus.Parse(a.Account.Status)
            })
            .FirstOrDefaultAsync(cancellationToken);

        return result!;
    }

    public Task PauseAccountAsync(int accountId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task ResumeAccountAsync(int accountId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateAccountAsync(UpdateAccount command, CancellationToken cancellationToken = default)
    {
        if (command == null)
            throw new ArgumentNullException(nameof(command));

        var account = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == command.Id, cancellationToken);
        if (account == null)
            throw new InvalidOperationException("Account not found.");

        account.Name = command.Name;
        account.Email = command.Email.Value;
        account.Phone = command.Phone;
        account.Address = command.Address;
        account.NumberId = command.NumberId;

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}