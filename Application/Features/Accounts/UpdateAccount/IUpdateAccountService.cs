using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.DTOs;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Requests;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.ValueObjects;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Abstractions;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Accounts.UpdateAccount;

public interface IUpdateAccountService
{
    Task<IGenericResponse<AccountInfoDTO>> ExecuteAsync(IdObject accountId, UpdateAccountRequest request);
}
