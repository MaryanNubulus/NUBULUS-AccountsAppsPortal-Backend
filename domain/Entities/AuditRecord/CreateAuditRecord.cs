using Nubulus.Domain.ValueObjects;

namespace Nubulus.Domain.Entities.AuditRecord;

public class CreateAuditRecord
{
    public string Key { get; set; } = string.Empty;

    public string TableName { get; set; } = string.Empty;

    public string RecordKey { get; set; } = string.Empty;

    public RecordType RecordType { get; set; } = RecordType.Create;

    public string User { get; set; } = string.Empty;

    public DateTime DateTime { get; set; } = DateTime.UtcNow;

    public string DataB64 { get; set; } = string.Empty;

    public CreateAuditRecord(string tableName, string recordKey, RecordType recordType, string user, string dataB64)
    {
        Key = Guid.NewGuid().ToString();
        TableName = tableName;
        RecordKey = recordKey;
        RecordType = recordType;
        User = user;
        DateTime = DateTime.UtcNow;
        DataB64 = dataB64;
    }
}