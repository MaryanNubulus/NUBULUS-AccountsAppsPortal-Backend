namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.DTOs;

public class CreateUserRequest
{
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}