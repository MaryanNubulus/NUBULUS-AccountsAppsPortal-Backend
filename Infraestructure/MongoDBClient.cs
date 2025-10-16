using MongoDB.Driver;

namespace NUBULUS.AccountsAppsPortalBackEnd.Infraestructure;

public class MongoDbClient : INoSQLClient
{
    private readonly IMongoDatabase _database;
    public const string ConnectionStringKey = "MongoDB";
    public const string DatabaseNameKey = "AccountsAppsPortalDb";

    public MongoDbClient(IConfiguration configuration)
    {
        var client = new MongoClient(configuration.GetConnectionString(ConnectionStringKey));
        _database = client.GetDatabase(DatabaseNameKey);
    }

    public IMongoCollection<T> GetCollection<T>(string name)
    {
        return _database.GetCollection<T>(name);
    }
}
