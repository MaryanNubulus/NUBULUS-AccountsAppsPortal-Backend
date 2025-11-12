using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Nubulus.Backend.Infraestructure.Pgsql.Models;

public class AccountUser
{
    public int Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string AccountKey { get; set; } = string.Empty;

    [JsonIgnore]
    public Account Account { get; set; } = default!;
    public string UserKey { get; set; } = string.Empty;

    [JsonIgnore]
    public User User { get; set; } = default!;
    public string Creator { get; set; } = "N";
    public string Status { get; set; } = "A";
}

internal sealed class AccountUserConfiguration : IEntityTypeConfiguration<AccountUser>
{
    public void Configure(EntityTypeBuilder<AccountUser> builder)
    {
        builder.ToTable("accounts_users");

        builder.HasIndex(au => au.Id);
        builder.Property(au => au.Id).HasColumnName("id").ValueGeneratedOnAdd();

        builder.HasKey(au => au.Key);
        builder.Property(au => au.Key).HasColumnName("key").IsRequired().HasMaxLength(36);

        builder.Property(au => au.AccountKey).HasColumnName("account_key").IsRequired().HasMaxLength(36);
        builder.Property(au => au.UserKey).HasColumnName("user_key").IsRequired().HasMaxLength(36);
        builder.Property(au => au.Creator).HasColumnName("creator").IsRequired().HasMaxLength(1);
        builder.Property(au => au.Status).HasColumnName("status").IsRequired().HasMaxLength(1);

        // Relaciones usando las claves
        builder.HasOne(au => au.Account)
            .WithMany(a => a.AccountUsers)
            .HasForeignKey(au => au.AccountKey)
            .HasPrincipalKey(a => a.Key);

        builder.HasOne(au => au.User)
            .WithMany(u => u.AccountUsers)
            .HasForeignKey(au => au.UserKey)
            .HasPrincipalKey(u => u.Key);
    }
}