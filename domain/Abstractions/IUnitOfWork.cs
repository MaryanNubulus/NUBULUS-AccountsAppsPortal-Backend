namespace Nubulus.Domain.Abstractions;

public interface IUnitOfWork : IDisposable
{
    IAccountsRepository Accounts { get; }
    IUsersRepository Users { get; }
    IAppsRepository Apps { get; }
    Task<int> CompleteAsync(CancellationToken cancellationToken = default);
}
