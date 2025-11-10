using Microsoft.EntityFrameworkCore;
using Nubulus.Backend.Infraestructure.Pgsql.Models;

namespace Nubulus.Backend.Infraestructure.Pgsql;

public class PostgreDBContext : DbContext
{
    public PostgreDBContext(DbContextOptions<PostgreDBContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new AccountConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new AccountUserConfiguration());
        modelBuilder.ApplyConfiguration(new AuditRecordConfiguration());
    }

    public DbSet<Account> Accounts { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<AccountUser> AccountUsers { get; set; }
    public DbSet<AuditRecord> AuditRecords { get; set; }

}
