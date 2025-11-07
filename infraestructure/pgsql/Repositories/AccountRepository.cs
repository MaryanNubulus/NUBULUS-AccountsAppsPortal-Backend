using Microsoft.EntityFrameworkCore;
using Nubulus.Backend.Infraestructure.PostgreSQL;
using Nubulus.Backend.Infraestructure.PostgreSQL.Models;
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

    public async Task<bool> AccountInfoExistsAsync(string name, string email, string phone, string numberId, CancellationToken cancellationToken = default)
    {
        var accountExists = await _dbContext.Accounts.AnyAsync(a =>
            a.Name == name ||
            a.Email == email ||
            a.Phone == phone ||
            a.NumberId == numberId, cancellationToken);

        if (accountExists) { return true; }

        var userExists = await _dbContext.Users.AnyAsync(u =>
            u.Email == email, cancellationToken);

        return userExists;
    }

    public async Task CreateAccountAsync(CreateAccount command, CancellationToken cancellationToken = default)
    {
        var account = new Account
        {
            Name = command.Name,
            Key = command.Key,
            Email = command.Email.Value,
            Phone = command.Phone,
            Address = command.Address,
            NumberId = command.NumberId,
            Status = "A"
        };

        var user = new User
        {
            Key = Guid.NewGuid().ToString(),
            Name = command.FullName,
            Email = command.Email.Value,
        };

        await _dbContext.Accounts.AddAsync(account, cancellationToken);
        await _dbContext.Users.AddAsync(user, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var accountUser = new AccountUser
        {
            AccountId = account.Id,
            UserId = user.Id,
            Role = "Owner",
            Shared = "N"
        };

        await _dbContext.AccountUsers.AddAsync(accountUser, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> CountAccountsAsync(string? searchTerm, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Accounts.AsNoTracking();

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
                        join au in _dbContext.AccountUsers on a.Id equals au.AccountId
                        where au.Role == "Owner"
                        join u in _dbContext.Users on au.UserId equals u.Id
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
    public Task<AccountEntity> GetAccountByIdAsync(int accountId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<AccountEntity> GetAccountByKeyAsync(string accountKey, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
    public Task PauseAccountAsync(int accountId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task ResumeAccountAsync(int accountId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAccountAsync(AccountEntity command, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}