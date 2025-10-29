using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.DTOs;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Responses;

public record GetAppsResponse(List<AppInfoDTO> Apps);