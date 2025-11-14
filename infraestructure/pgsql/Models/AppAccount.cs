using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Nubulus.Backend.Infraestructure.Pgsql.Models;

public class AppAccount
{
    public int Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string AppKey { get; set; } = default!;

    [JsonIgnore]
    public App App { get; set; } = null!;

    public string AccountKey { get; set; } = default!;

    [JsonIgnore]
    public Account Account { get; set; } = null!;

    public string Status { get; set; } = default!;

    [JsonIgnore]
    public ICollection<AppToken> AppTokens { get; set; } = new List<AppToken>();
}

internal sealed class AppAccountConfiguration : IEntityTypeConfiguration<AppAccount>
{
    public void Configure(EntityTypeBuilder<AppAccount> builder)
    {
        builder.ToTable("app_accounts");
        builder.HasIndex(aa => aa.Id);
        builder.Property(aa => aa.Id).HasColumnName("id").ValueGeneratedOnAdd();

        builder.HasKey(aa => aa.Key);
        builder.Property(aa => aa.Key).HasColumnName("key").IsRequired().HasMaxLength(36);

        builder.Property(aa => aa.AppKey).HasColumnName("app_key").IsRequired().HasMaxLength(36);
        builder.Property(aa => aa.AccountKey).HasColumnName("account_key").IsRequired().HasMaxLength(36);
        builder.Property(aa => aa.Status).HasColumnName("status").IsRequired().HasMaxLength(1);

        builder.HasOne(aa => aa.App)
            .WithMany(a => a.AppAccounts)
            .HasForeignKey(aa => aa.AppKey);

        builder.HasOne(aa => aa.Account)
            .WithMany(a => a.AppAccounts)
            .HasForeignKey(aa => aa.AccountKey);

        builder.HasMany(aa => aa.AppTokens)
            .WithOne(at => at.AppAccount)
            .HasForeignKey(at => at.AppAccountKey);
    }
}