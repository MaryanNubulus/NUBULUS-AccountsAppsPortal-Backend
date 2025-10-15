using MongoDB.Driver;

namespace NUBULUS.AccountsAppsPortalBackEnd.Infraestructure;

public interface INoSQLClient
{
    IMongoCollection<T> GetCollection<T>(string name);
}
