
using Nubulus.Domain.Abstractions;

namespace Nubulus.Backend.Infraestructure.Pgsql;

public class UnitOfWork : IUnitOfWork
{
    private readonly PostgreDBContext _context;

    public UnitOfWork(PostgreDBContext context)
    {
        _context = context;
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}

