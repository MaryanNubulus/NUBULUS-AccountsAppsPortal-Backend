using Nubulus.Domain.Entities.AuditRecord;

namespace Nubulus.Domain.Abstractions;

public interface IAuditRecordRepository
{
    Task CreateAuditRecordAsync(CreateAuditRecord command, CancellationToken cancellationToken = default);
}