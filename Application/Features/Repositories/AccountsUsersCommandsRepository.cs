using MongoDB.Driver;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Abstractions;
using NUBULUS.AccountsAppsPortalBackEnd.Domain.Entities;
using NUBULUS.AccountsAppsPortalBackEnd.Infraestructure;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Repositories;

public class AccountsUsersCommandsRepository : IAccountsUsersCommandsRepository
{
    private readonly INoSQLClient _noSQLClient;
    private readonly IMongoCollection<AccountsUsers> _collection;
    private const string CollectionName = $"{nameof(AccountsUsers)}s";
    public AccountsUsersCommandsRepository(INoSQLClient noSQLClient)
    {
        _noSQLClient = noSQLClient;
        _collection = _noSQLClient.GetCollection<AccountsUsers>(CollectionName);
    }
    public async Task<bool> AddAsync(AccountsUsers entity)
    {
        await _collection.InsertOneAsync(entity);
        return _collection.Find(x => x.Id == entity.Id).Any();
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        await _collection.DeleteOneAsync(x => x.Id == id);
        return !_collection.Find(x => x.Id == id).Any();
    }

    public async Task<bool> UpdateAsync(Guid id, AccountsUsers entity)
    {
        await _collection.ReplaceOneAsync(x => x.Id == id, entity);
        return _collection.Find(x => x.Id == id).Any();
    }
}