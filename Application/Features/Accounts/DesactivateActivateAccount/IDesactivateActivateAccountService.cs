using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.DTOs;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.ValueObjects;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Abstractions;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Accounts.DesactivateActivateAccount;

public interface IDesactivateActivateAccountService
{
    Task<IGenericResponse<AccountInfoDTO>> ExecuteAsync(IdObject accountId, bool activate);
}
