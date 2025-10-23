using MongoDB.Driver.Linq;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Abstractions;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Apps.ExistKeyApp;

public class ExistKeyAppService : IExistKeyAppService
{
    private readonly IAppsQueriesRepository _appsQueriesRepository;

    public ExistKeyAppService(IAppsQueriesRepository appsQueriesRepository)
    {
        _appsQueriesRepository = appsQueriesRepository;
    }

    public async Task<bool> ExistKeyAppAsync(string appKey)
    {
        return await _appsQueriesRepository.GetAll().FirstOrDefaultAsync(a => a.Key == appKey) != null;
    }
}