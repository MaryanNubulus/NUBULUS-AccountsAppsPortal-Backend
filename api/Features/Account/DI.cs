using Nubulus.Backend.Api.Features.Account.CreateAccount;
using Nubulus.Backend.Infraestructure.Pgsql.Repositories;
using Nubulus.Domain.Abstractions;

namespace Nubulus.Backend.Api.Features.Account;

public static class DI
{
    public static IServiceCollection AddAccountFeature(this IServiceCollection services)
    {
        services.AddScoped<CreateAccountService>();
        services.AddScoped<IAccountsRepository, AccountRepository>();
        return services;
    }

    public static WebApplication MapAccountEndpoints(this WebApplication app)
    {
        app.MapCreateAccountEndPoint();
        return app;
    }
}