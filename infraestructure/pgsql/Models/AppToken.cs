using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Nubulus.Backend.Infraestructure.Pgsql.Models;

public class AppToken
{
    public int Id { get; set; }

    public string Key { get; set; } = string.Empty;

    public string AppAccountKey { get; set; } = string.Empty;

    [JsonIgnore]
    public AppAccount AppAccount { get; set; } = null!;

    public string Token { get; set; } = string.Empty;

    public DateTime Expiration { get; set; }

    public string Status { get; set; } = "A";
}

internal sealed class AppTokenConfiguration : IEntityTypeConfiguration<AppToken>
{
    public void Configure(EntityTypeBuilder<AppToken> builder)
    {
        builder.ToTable("app_tokens");

        builder.HasIndex(at => at.Id);
        builder.Property(at => at.Id).HasColumnName("id").ValueGeneratedOnAdd();

        builder.HasKey(at => at.Key);
        builder.Property(at => at.Key).HasColumnName("key").IsRequired().HasMaxLength(36);

        builder.Property(at => at.AppAccountKey).HasColumnName("app_account_key").IsRequired().HasMaxLength(36);

        builder.Property(at => at.Token).HasColumnName("token").IsRequired().HasMaxLength(256);

        builder.Property(at => at.Expiration).HasColumnName("expiration").IsRequired();

        builder.Property(at => at.Status).HasColumnName("status").IsRequired().HasMaxLength(1);

        builder.HasOne(at => at.AppAccount)
            .WithMany(aa => aa.AppTokens)
            .HasForeignKey(at => at.AppAccountKey);
    }
}