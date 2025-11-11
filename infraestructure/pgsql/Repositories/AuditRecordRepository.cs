using Nubulus.Backend.Infraestructure.Pgsql.Models;
using Nubulus.Domain.Abstractions;

namespace Nubulus.Backend.Infraestructure.Pgsql.Repositories;

public class AuditRecordRepository
{
    private readonly PostgreDBContext _dbContext;
    public AuditRecordRepository(PostgreDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CreateAuditRecordAsync(AuditRecord auditRecord, CancellationToken cancellationToken = default)
    {
        await _dbContext.AuditRecords.AddAsync(auditRecord, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}