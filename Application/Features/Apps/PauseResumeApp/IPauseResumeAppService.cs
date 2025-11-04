using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.DTOs;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.ValueObjects;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Abstractions;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Apps.PauseResumeApp;

public interface IPauseResumeAppService
{
    Task<IGenericResponse<AppInfoDTO>> ExecuteAsync(IdObject id, bool isActive);
}
