using Nubulus.Backend.Infraestructure.Pgsql.Models;
using Nubulus.Domain.Abstractions;
using Nubulus.Domain.Entities.AuditRecord;

namespace Nubulus.Backend.Infraestructure.Pgsql.Repositories;

public class AuditRecordRepository : IAuditRecordRepository
{
    private readonly PostgreDBContext _dbContext;
    public AuditRecordRepository(PostgreDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CreateAuditRecordAsync(CreateAuditRecord command, CancellationToken cancellationToken = default)
    {
        var auditRecord = new AuditRecord
        {
            Key = command.Key,
            TableName = command.TableName,
            RecordKey = command.RecordKey,
            RecordType = command.RecordType.ToString(),
            User = command.User,
            DateTime = command.DateTime,
            DataB64 = command.DataB64
        };

        await _dbContext.AuditRecords.AddAsync(auditRecord, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}