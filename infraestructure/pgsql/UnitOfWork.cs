
using Nubulus.Domain.Abstractions;

namespace Nubulus.Backend.Infraestructure.Pgsql;

public class UnitOfWork : IUnitOfWork
{
    private readonly PostgreDBContext _context;
    private readonly IAccountsRepository _accountsRepository;
    public IAccountsRepository Accounts => _accountsRepository;

    public UnitOfWork(PostgreDBContext context, IAccountsRepository accountsRepository)
    {
        _context = context;
        _accountsRepository = accountsRepository;
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

