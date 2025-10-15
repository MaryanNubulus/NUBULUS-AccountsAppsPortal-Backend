namespace NUBULUS.AccountsAppsPortalBackEnd.Infraestructure;
public static class DI
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<INoSQLClient, MongoDbClient>();

        return services;
    }
}