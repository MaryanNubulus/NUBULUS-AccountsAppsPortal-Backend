using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Nubulus.Backend.Infraestructure.Pgsql.Models;

public class User
{
    public int Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string ParentKey { get; set; } = string.Empty;  // Key del Account on s'ha creat
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Status { get; set; } = "A";

    [JsonIgnore]
    public ICollection<AccountUser> AccountUsers { get; set; } = new List<AccountUser>();
}

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasIndex(u => u.Id);
        builder.Property(u => u.Id).HasColumnName("id").ValueGeneratedOnAdd();

        builder.HasKey(u => u.Key);
        builder.Property(u => u.Key).HasColumnName("key").IsRequired().HasMaxLength(36);

        builder.Property(u => u.ParentKey).HasColumnName("parent_key").IsRequired().HasMaxLength(36);
        builder.Property(u => u.FullName).HasColumnName("full_name").IsRequired().HasMaxLength(100);
        builder.Property(u => u.Email).HasColumnName("email").IsRequired().HasMaxLength(100);
        builder.Property(u => u.Phone).HasColumnName("phone").IsRequired().HasMaxLength(15);
        builder.Property(u => u.Password).HasColumnName("password").HasMaxLength(200);
        builder.Property(u => u.Status).HasColumnName("status").IsRequired().HasMaxLength(1);
    }
}
