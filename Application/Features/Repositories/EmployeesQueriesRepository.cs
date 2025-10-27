using MongoDB.Driver;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Abstractions;
using NUBULUS.AccountsAppsPortalBackEnd.Domain.Entities;
using NUBULUS.AccountsAppsPortalBackEnd.Infraestructure;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Repositories;

public class EmployeesQueriesRepository : IEmployeesQueriesRepository
{
    private readonly INoSQLClient _noSQLClient;
    private readonly IMongoCollection<Employee> _collection;
    private const string CollectionName = $"{nameof(Employee)}s";

    public EmployeesQueriesRepository(INoSQLClient noSQLClient)
    {
        _noSQLClient = noSQLClient;
        _collection = _noSQLClient.GetCollection<Employee>(CollectionName);
    }
    public IQueryable<Employee> GetAll()
    {
        return _collection.AsQueryable();
    }

    public Task<Employee> GetOneAsync(Guid id)
    {
        return _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
    }
}
