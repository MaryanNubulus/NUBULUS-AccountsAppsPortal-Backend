namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.DTOs;

public record AccountInfoDTO
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string UserEmail { get; init; } = string.Empty;
    public string UserPhone { get; init; } = string.Empty;
}