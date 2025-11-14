using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Nubulus.Backend.Infraestructure.Pgsql.Models;

public class App
{
    public int Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = "A";

    [JsonIgnore]
    public ICollection<AppAccount> AppAccounts { get; set; } = new List<AppAccount>();
}

internal sealed class AppConfiguration : IEntityTypeConfiguration<App>
{
    public void Configure(EntityTypeBuilder<App> builder)
    {
        builder.ToTable("apps");

        builder.HasIndex(a => a.Id);
        builder.Property(a => a.Id).HasColumnName("id").ValueGeneratedOnAdd();

        builder.HasKey(a => a.Key);
        builder.Property(a => a.Key).HasColumnName("key").IsRequired().HasMaxLength(100);

        builder.Property(a => a.Name).HasColumnName("name").IsRequired().HasMaxLength(100);
        builder.Property(a => a.Status).HasColumnName("status").IsRequired().HasMaxLength(1);

        builder.HasMany(a => a.AppAccounts)
            .WithOne(aa => aa.App)
            .HasForeignKey(aa => aa.AppKey);
    }
}
