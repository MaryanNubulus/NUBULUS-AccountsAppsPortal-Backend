using Microsoft.EntityFrameworkCore;
using Nubulus.Backend.Infraestructure.PostgreSQL;
using Nubulus.Backend.Infraestructure.PostgreSQL.Models;
using Nubulus.Domain.Abstractions;
using Nubulus.Domain.Entities.Account;

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

    public Task<AccountEntity> GetAccountByIdAsync(int accountId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<AccountEntity> GetAccountByKeyAsync(string accountKey, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IQueryable<AccountEntity>> GetAllAccountsAsync(CancellationToken cancellationToken = default)
    {
        // Retornar IQueryable de entidades de dominio
        // NOTA: Este método tiene limitaciones con paginación debido a que EF Core
        // no puede traducir la creación de Value Objects a SQL
        // Se recomienda usar GetAllAccountsQueryable() para paginación
        var accountEntities = _dbContext.Accounts
            .AsNoTracking()
            .AsEnumerable() // Materializar primero
            .Select(a => new AccountEntity
            {
                Id = a.Id,
                AccountKey = new Domain.ValueObjects.AccountKey(a.Key),
                Name = a.Name,
                Email = new Domain.ValueObjects.EmailAddress(a.Email),
                Phone = a.Phone,
                Status = a.Status == "A"
                    ? Domain.ValueObjects.AccountStatus.Active
                    : Domain.ValueObjects.AccountStatus.Inactive
            })
            .AsQueryable();

        return Task.FromResult(accountEntities);
    }

    // Método auxiliar para paginación que retorna IQueryable<Account> (modelo de persistencia)
    public IQueryable<Account> GetAllAccountsQueryable()
    {
        return _dbContext.Accounts.AsNoTracking();
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