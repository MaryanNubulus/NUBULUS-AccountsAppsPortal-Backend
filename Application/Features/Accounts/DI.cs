namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Accounts;

public static class DI
{
    public static IServiceCollection AddAccountsServices(this IServiceCollection services)
    {
        // Add account-related services here

        return services;
    }

    public static WebApplication MapAccountsEndpoints(this WebApplication app)
    {
        // Map account-related endpoints here

        return app;
    }
}