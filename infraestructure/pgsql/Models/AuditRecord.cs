using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Nubulus.Backend.Infraestructure.Pgsql.Models;

public class AuditRecord
{
    public int Id { get; set; }

    public string Key { get; set; } = string.Empty;

    public string TableName { get; set; } = string.Empty;

    public string RecordKey { get; set; } = string.Empty;

    public string RecordType { get; set; } = string.Empty;

    public string User { get; set; } = string.Empty;

    public DateTime DateTime { get; set; } = DateTime.UtcNow;

    public string DataB64 { get; set; } = string.Empty;
}

internal sealed class AuditRecordConfiguration : IEntityTypeConfiguration<AuditRecord>
{
    public void Configure(EntityTypeBuilder<AuditRecord> builder)
    {
        builder.ToTable("audit_records");

        builder.HasIndex(ar => ar.Id);
        builder.Property(ar => ar.Id).HasColumnName("id").ValueGeneratedOnAdd();

        builder.HasKey(ar => ar.Key);
        builder.Property(ar => ar.Key).HasColumnName("key").IsRequired().HasMaxLength(36);

        builder.Property(ar => ar.TableName).HasColumnName("table_name").IsRequired().HasMaxLength(100);
        builder.Property(ar => ar.RecordKey).HasColumnName("record_key").IsRequired().HasMaxLength(36);
        builder.Property(ar => ar.RecordType).HasColumnName("record_type").IsRequired().HasMaxLength(1);
        builder.Property(ar => ar.User).HasColumnName("user").IsRequired().HasMaxLength(264);
        builder.Property(ar => ar.DateTime).HasColumnName("date_time").IsRequired();
        builder.Property(ar => ar.DataB64).HasColumnName("data_b64").IsRequired();
    }
}