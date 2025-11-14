using Microsoft.EntityFrameworkCore;
using Nubulus.Backend.Infraestructure.Pgsql.Mappers;
using Nubulus.Backend.Infraestructure.Pgsql.Models;
using Nubulus.Domain.Abstractions;
using Nubulus.Domain.Entities.App;
using Nubulus.Domain.ValueObjects;

namespace Nubulus.Backend.Infraestructure.Pgsql.Repositories;

public class AppRepository : IAppsRepository
{
    private readonly PostgreDBContext _dbContext;

    public AppRepository(PostgreDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> AppKeyExistsAsync(AppKey key, CancellationToken cancellationToken = default, AppId? excludeAppId = null)
    {
        var keyExists = await _dbContext.Apps.AnyAsync(a =>
            a.Key == key.Value &&
            (excludeAppId == null || a.Id != excludeAppId.Value),
            cancellationToken);

        return keyExists;
    }

    public async Task<int> CountAppsAsync(string? searchTerm, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Apps.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(a =>
                a.Name.ToUpper().Contains(searchTerm.ToUpper()) ||
                a.Key.ToUpper().Contains(searchTerm.ToUpper()));
        }

        return await query.CountAsync(cancellationToken);
    }

    public async Task<IQueryable<AppEntity>> GetAppsAsync(string? searchTerm, int? page, int? size, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Apps.OrderBy(a => a.Id).AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(a =>
                a.Name.ToUpper().Contains(searchTerm.ToUpper()) ||
                a.Key.ToUpper().Contains(searchTerm.ToUpper()));
        }

        if (page.HasValue && size.HasValue)
        {
            query = query.Skip((page.Value - 1) * size.Value).Take(size.Value);
        }

        var results = await query.Select(a => new AppEntity
        {
            AppId = new AppId(a.Id),
            AppKey = new AppKey(a.Key),
            Name = a.Name,
            Status = Status.Parse(a.Status)
        }).ToListAsync(cancellationToken);

        return results.AsQueryable();
    }

    public async Task<AppEntity> GetAppByIdAsync(AppId appId, CancellationToken cancellationToken = default)
    {
        var result = await _dbContext.Apps
            .AsNoTracking()
            .Where(a => a.Id == appId.Value)
            .Select(a => new AppEntity
            {
                AppId = new AppId(a.Id),
                AppKey = new AppKey(a.Key),
                Name = a.Name,
                Status = Status.Parse(a.Status)
            })
            .FirstOrDefaultAsync(cancellationToken);

        return result!;
    }

    public async Task<AppEntity> GetAppByKeyAsync(AppKey appKey, CancellationToken cancellationToken = default)
    {
        var result = await _dbContext.Apps
            .AsNoTracking()
            .Where(a => a.Key == appKey.Value)
            .Select(a => new AppEntity
            {
                AppId = new AppId(a.Id),
                AppKey = new AppKey(a.Key),
                Name = a.Name,
                Status = Status.Parse(a.Status)
            })
            .FirstOrDefaultAsync(cancellationToken);

        return result!;
    }

    public async Task CreateAppAsync(CreateApp command, EmailAddress currentUserEmail, CancellationToken cancellationToken = default)
    {
        var app = new App
        {
            Key = command.AppKey.Value,
            Name = command.Name,
            Status = "A"
        };

        var appAuditRecord = app.ToAuditRecord(currentUserEmail.Value, RecordType.Create);

        await _dbContext.Apps.AddAsync(app, cancellationToken);
        await _dbContext.AuditRecords.AddAsync(appAuditRecord, cancellationToken);
    }

    public async Task PauseAppAsync(AppId appId, EmailAddress currentUserEmail, CancellationToken cancellationToken = default)
    {
        var app = await _dbContext.Apps.FirstOrDefaultAsync(a => a.Id == appId.Value, cancellationToken);
        if (app == null)
            throw new InvalidOperationException("App not found.");

        app.Status = "I";

        await _dbContext.AuditRecords.AddAsync(app.ToAuditRecord(currentUserEmail.Value, RecordType.Pause), cancellationToken);
    }

    public async Task ResumeAppAsync(AppId appId, EmailAddress currentUserEmail, CancellationToken cancellationToken = default)
    {
        var app = await _dbContext.Apps.FirstOrDefaultAsync(a => a.Id == appId.Value, cancellationToken);
        if (app == null)
            throw new InvalidOperationException("App not found.");

        app.Status = "A";

        await _dbContext.AuditRecords.AddAsync(app.ToAuditRecord(currentUserEmail.Value, RecordType.Resume), cancellationToken);
    }

    public async Task UpdateAppAsync(UpdateApp command, EmailAddress currentUserEmail, CancellationToken cancellationToken = default)
    {
        if (command == null)
            throw new ArgumentNullException(nameof(command));

        var app = await _dbContext.Apps.FirstOrDefaultAsync(a => a.Id == command.AppId.Value, cancellationToken);
        if (app == null)
            throw new InvalidOperationException("App not found.");

        app.Name = command.Name;

        await _dbContext.AuditRecords.AddAsync(app.ToAuditRecord(currentUserEmail.Value, RecordType.Update), cancellationToken);
    }
}
