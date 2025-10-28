using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Repositories;

namespace NUBULUS.AccountsAppsPortalBackEnd.Domain.Entities;

public class Account
{
    public AccountId Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

