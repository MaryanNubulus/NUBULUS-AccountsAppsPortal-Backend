namespace Nubulus.Domain.Abstractions;

public interface IUnitOfWork : IDisposable
{
    IAccountsRepository Accounts { get; }
    Task<int> CompleteAsync(CancellationToken cancellationToken = default);
}
