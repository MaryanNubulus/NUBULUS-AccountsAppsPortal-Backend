namespace NUBULUS.AccountsAppsPortalBackEnd.Domain.Abstractions;

public interface IGenericQueriesRepository<TId, TResult> where TId : struct where TResult : class
{

    IQueryable<TResult> GetAll();

    Task<TResult> GetOneAsync(TId id);
}
