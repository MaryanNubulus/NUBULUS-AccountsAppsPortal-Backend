namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Apps.ExistKeyApp;

public interface IExistKeyAppService
{
    Task<bool> ExistKeyAppAsync(string appKey);
}
