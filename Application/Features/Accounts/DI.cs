using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Accounts.CreateAccount;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Accounts.GetAccounts;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Accounts.DesactivateActivateAccount;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Accounts.UpdateAccount;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Accounts;

public static class DI
{
    public static IServiceCollection AddAccountsServices(this IServiceCollection services)
    {
        services.AddTransient<ICreateAccountService, CreateAccountService>();
        services.AddTransient<IGetAccountsService, GetAccountsService>();
        services.AddTransient<IDesactivateActivateAccountService, DesactivateActivateAccountService>();
        services.AddTransient<IUpdateAccountService, UpdateAccountService>();

        return services;
    }

    public static WebApplication MapAccountsEndpoints(this WebApplication app)
    {
        app.MapCreateAccountEndpoint();
        app.MapGetAccountsEndpoint();
        app.MapDesactivateActivateAccountEndpoint();
        app.MapUpdateAccountEndpoint();

        return app;
    }
}