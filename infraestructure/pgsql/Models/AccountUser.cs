using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Nubulus.Backend.Infraestructure.PostgreSQL.Models;

public class AccountUser
{
    public int Id { get; set; }
    public int AccountId { get; set; }
    public Account Account { get; set; } = default!;
    public int UserId { get; set; }
    public User User { get; set; } = default!;
    public string Role { get; set; } = "User";
    public string Shared { get; set; } = "N";
}

internal sealed class AccountUserConfiguration : IEntityTypeConfiguration<AccountUser>
{
    public void Configure(EntityTypeBuilder<AccountUser> builder)
    {
        builder.ToTable("accounts_users");

        builder.HasKey(au => au.Id);
        builder.Property(au => au.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(au => au.AccountId).HasColumnName("account_id").IsRequired();
        builder.Property(au => au.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(au => au.Role).HasColumnName("role").IsRequired().HasMaxLength(20);
        builder.Property(au => au.Shared).HasColumnName("shared").IsRequired().HasMaxLength(1);

        builder.HasKey(au => new { au.AccountId, au.UserId });

        builder.HasOne(au => au.Account)
            .WithMany(a => a.AccountUsers)
            .HasForeignKey(au => au.AccountId);

        builder.HasOne(au => au.User)
            .WithMany(u => u.AccountUsers)
            .HasForeignKey(au => au.UserId);

    }
}