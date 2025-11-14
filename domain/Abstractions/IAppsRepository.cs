using Nubulus.Domain.Entities.App;
using Nubulus.Domain.ValueObjects;

namespace Nubulus.Domain.Abstractions;

public interface IAppsRepository
{
    Task<AppEntity> GetAppByKeyAsync(AppKey appKey, CancellationToken cancellationToken = default);
    Task<AppEntity> GetAppByIdAsync(AppId appId, CancellationToken cancellationToken = default);
    Task<int> CountAppsAsync(string? searchTerm, CancellationToken cancellationToken = default);
    Task<IQueryable<AppEntity>> GetAppsAsync(string? searchTerm, int? page, int? size, CancellationToken cancellationToken = default);
    Task<bool> AppKeyExistsAsync(AppKey key, CancellationToken cancellationToken = default, AppId? excludeAppId = null);
    Task CreateAppAsync(CreateApp command, EmailAddress currentUserEmail, CancellationToken cancellationToken = default);
    Task UpdateAppAsync(UpdateApp command, EmailAddress currentUserEmail, CancellationToken cancellationToken = default);
    Task PauseAppAsync(AppId appId, EmailAddress currentUserEmail, CancellationToken cancellationToken = default);
    Task ResumeAppAsync(AppId appId, EmailAddress currentUserEmail, CancellationToken cancellationToken = default);
}
