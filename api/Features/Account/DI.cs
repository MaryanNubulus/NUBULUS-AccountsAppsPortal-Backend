using Nubulus.Backend.Api.Features.Account.CreateAccount;
using Nubulus.Backend.Api.Features.Account.GetAccount;
using Nubulus.Backend.Api.Features.Account.GetAccounts;
using Nubulus.Backend.Api.Features.Account.PauseResumeAccount;
using Nubulus.Backend.Api.Features.Account.UpdateAccount;

namespace Nubulus.Backend.Api.Features.Account;

public static class DI
{
    public static IServiceCollection AddAccountFeature(this IServiceCollection services)
    {
        services.AddTransient<CreateAccountService>();
        services.AddTransient<GetAccountsService>();
        services.AddTransient<GetAccountService>();
        services.AddTransient<UpdateAccountService>();
        services.AddTransient<PauseResumeAccountService>();
        return services;
    }

    public static WebApplication MapAccountEndpoints(this WebApplication app)
    {
        app.MapCreateAccountEndPoint();
        app.MapGetAccountsEndPoint();
        app.MapGetAccountEndPoint();
        app.MapUpdateAccountEndPoint();
        app.MapPauseResumeAccountEndPoint();
        return app;
    }
}