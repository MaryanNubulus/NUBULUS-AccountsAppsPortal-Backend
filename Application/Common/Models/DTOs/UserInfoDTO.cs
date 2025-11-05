namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.DTOs;

public record UserInfoDTO
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public string Role { get; init; } = string.Empty;
}
