using Nubulus.Backend.Infraestructure.Pgsql.Models;
using Nubulus.Domain.ValueObjects;

namespace Nubulus.Backend.Infraestructure.Pgsql.Mappers;

public static class AppAuditRecord
{
    public static AuditRecord ToAuditRecord(this App entity, string author, RecordType recordType)
    {
        return new AuditRecord
        {
            Key = Guid.NewGuid().ToString(),
            TableName = $"{nameof(App)}s".ToLower(),
            RecordKey = entity.Key,
            RecordType = recordType.Value,
            User = author,
            DateTime = DateTime.UtcNow,
            DataB64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(entity)))
        };
    }
}
