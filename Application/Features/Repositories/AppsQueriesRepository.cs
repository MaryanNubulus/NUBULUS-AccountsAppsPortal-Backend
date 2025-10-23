using MongoDB.Driver;
using MongoDB.Driver.Linq;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Abstractions;
using NUBULUS.AccountsAppsPortalBackEnd.Domain.Entities;
using NUBULUS.AccountsAppsPortalBackEnd.Infraestructure;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Repositories;

public class AppsQueriesRepository : IAppsQueriesRepository
{
    private readonly INoSQLClient _noSQLClient;
    private readonly IMongoCollection<App> _collection;
    private const string CollectionName = $"{nameof(App)}s";

    public AppsQueriesRepository(INoSQLClient noSQLClient)
    {
        _noSQLClient = noSQLClient;
        _collection = _noSQLClient.GetCollection<App>(CollectionName);
    }
    public IQueryable<App> GetAll()
    {
        return _collection.AsQueryable();
    }

    public async Task<App> GetOneAsync(Guid id)
    {
        return await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
    }
}