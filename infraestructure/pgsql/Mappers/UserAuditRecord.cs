using Nubulus.Backend.Infraestructure.Pgsql.Models;
using Nubulus.Domain.ValueObjects;

namespace Nubulus.Backend.Infraestructure.Pgsql.Mappers;

public static class UserAuditRecord
{
    public static AuditRecord ToAuditRecord(this User entity, string author, RecordType recordType)
    {
        return new AuditRecord
        {
            Key = Guid.NewGuid().ToString(),
            TableName = $"{nameof(User)}s".ToLower(),
            RecordKey = entity.Key,
            RecordType = recordType.Value,
            User = author,
            DateTime = DateTime.UtcNow,
            DataB64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(entity)))
        };
    }
}