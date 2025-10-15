namespace NUBULUS.AccountsAppsPortalBackEnd.Domain.Abstractions;

public interface IGenericCommandsRepository<TId, TResult> where TId : struct where TResult : class
{
    Task<bool> AddAsync(TResult entity);

    Task<bool> UpdateAsync(TId id, TResult entity);

    Task<bool> DeleteAsync(TId id);
}
