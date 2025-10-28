using MongoDB.Driver;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Abstractions;
using NUBULUS.AccountsAppsPortalBackEnd.Domain.Entities;
using NUBULUS.AccountsAppsPortalBackEnd.Infraestructure;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Repositories;

public class UsersQueriesRepository : IUsersQueriesRepository
{
    private readonly INoSQLClient _noSQLClient;
    private readonly IMongoCollection<User> _collection;
    private const string CollectionName = $"{nameof(User)}s";

    public UsersQueriesRepository(INoSQLClient noSQLClient)
    {
        _noSQLClient = noSQLClient;
        _collection = _noSQLClient.GetCollection<User>(CollectionName);
    }

    public IQueryable<User> GetAll()
    {
        return _collection.AsQueryable();
    }

    public Task<User> GetOneAsync(Guid id)
    {
        return _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
    }
}
