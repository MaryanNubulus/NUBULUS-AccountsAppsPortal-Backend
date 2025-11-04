using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.DTOs;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Abstractions;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Accounts.GetAccounts;

public interface IGetAccountsService
{
    Task<IGenericResponse<IEnumerable<AccountInfoDTO>>> ExecuteAsync();
}
