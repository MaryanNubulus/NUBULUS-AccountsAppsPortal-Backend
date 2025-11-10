using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Nubulus.Backend.Infraestructure.Pgsql.Models;

public class User
{
    public int Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public ICollection<AccountUser> AccountUsers { get; set; } = new List<AccountUser>();
}

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).HasColumnName("id").ValueGeneratedOnAdd();

        // Configurar Key como clave alternativa única
        builder.HasAlternateKey(u => u.Key);
        builder.Property(u => u.Key).HasColumnName("key").IsRequired().HasMaxLength(36);

        builder.Property(u => u.Name).HasColumnName("name").IsRequired().HasMaxLength(100);
        builder.Property(u => u.Email).HasColumnName("email").IsRequired().HasMaxLength(100);
    }
}
