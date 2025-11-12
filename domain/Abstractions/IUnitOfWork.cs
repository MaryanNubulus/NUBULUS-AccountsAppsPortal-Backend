namespace Nubulus.Domain.Abstractions;

public interface IUnitOfWork : IDisposable
{
    IAccountsRepository Accounts { get; }
    IUsersRepository Users { get; }
    Task<int> CompleteAsync(CancellationToken cancellationToken = default);
}
