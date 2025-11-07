using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Nubulus.Backend.Infraestructure.PostgreSQL.Models;

public class Account
{

    public int Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string NumberId { get; set; } = string.Empty;
    public string Status { get; set; } = "A";
    public ICollection<AccountUser> AccountUsers { get; set; } = new List<AccountUser>();
}

internal sealed class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("accounts");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(a => a.Key).HasColumnName("key").IsRequired().HasMaxLength(36);
        builder.Property(a => a.Name).HasColumnName("name").IsRequired().HasMaxLength(100);
        builder.Property(a => a.Email).HasColumnName("email").IsRequired().HasMaxLength(100);
        builder.Property(a => a.Phone).HasColumnName("phone").IsRequired().HasMaxLength(15);
        builder.Property(a => a.Address).HasColumnName("address").IsRequired().HasMaxLength(200);
        builder.Property(a => a.NumberId).HasColumnName("number_id").IsRequired().HasMaxLength(50);
        builder.Property(a => a.Status).HasColumnName("status").IsRequired().HasMaxLength(1);
    }
}
