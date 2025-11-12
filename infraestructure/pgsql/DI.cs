using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nubulus.Backend.Infraestructure.Pgsql.Repositories;
using Nubulus.Domain.Abstractions;

namespace Nubulus.Backend.Infraestructure.Pgsql;

public static class DI
{
    public static IServiceCollection AddPgsqlInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PostgreDBContext>(options =>
            options.UseNpgsql(
            configuration.GetConnectionString("PostgreSQLConnection"),
            b => b.MigrationsAssembly("nubulus.backend.api")
        ));

        //Implementación de UnitOfWork
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IAccountsRepository, AccountRepository>();
        services.AddScoped<IUsersRepository, UserRepository>();

        return services;
    }
}