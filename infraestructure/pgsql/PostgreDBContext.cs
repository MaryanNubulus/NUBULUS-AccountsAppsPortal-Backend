using Microsoft.EntityFrameworkCore;
using Nubulus.Backend.Infraestructure.PostgreSQL.Models;

namespace Nubulus.Backend.Infraestructure.PostgreSQL;

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
    }

    public DbSet<Account> Accounts { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<AccountUser> AccountUsers { get; set; }

}
