using MongoDB.Driver;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Abstractions;
using NUBULUS.AccountsAppsPortalBackEnd.Domain.Entities;
using NUBULUS.AccountsAppsPortalBackEnd.Infraestructure;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Repositories;

public class AccountsUsersQueriesRepository : IAccountsUsersQueriesRepository
{
    private readonly INoSQLClient _noSQLClient;
    private readonly IMongoCollection<AccountsUsers> _collection;
    private const string CollectionName = $"{nameof(AccountsUsers)}s";

    public AccountsUsersQueriesRepository(INoSQLClient noSQLClient)
    {
        _noSQLClient = noSQLClient;
        _collection = _noSQLClient.GetCollection<AccountsUsers>(CollectionName);
    }
    public IQueryable<AccountsUsers> GetAll()
    {
        return _collection.AsQueryable();
    }

    public Task<AccountsUsers> GetOneAsync(Guid id)
    {
        return _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
    }
}
