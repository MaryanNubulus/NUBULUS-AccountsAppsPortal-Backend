
using Nubulus.Domain.Abstractions;

namespace Nubulus.Backend.Infraestructure.Pgsql;

public class UnitOfWork : IUnitOfWork
{
    private readonly PostgreDBContext _context;
    private readonly IAccountsRepository _accountsRepository;
    private readonly IUsersRepository _usersRepository;
    private readonly IAppsRepository _appsRepository;

    public IAccountsRepository Accounts => _accountsRepository;
    public IUsersRepository Users => _usersRepository;
    public IAppsRepository Apps => _appsRepository;

    public UnitOfWork(PostgreDBContext context, IAccountsRepository accountsRepository, IUsersRepository usersRepository, IAppsRepository appsRepository)
    {
        _context = context;
        _accountsRepository = accountsRepository;
        _usersRepository = usersRepository;
        _appsRepository = appsRepository;
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    public async Task<int> CompleteAsync(CancellationToken cancellationToken = default)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var result = await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return result;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}

