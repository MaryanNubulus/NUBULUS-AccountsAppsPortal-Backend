using Nubulus.Domain.ValueObjects;

namespace Nubulus.Domain.Entities.Account;

public class AccountEntity
{
    public int Id { get; set; }
    public AccountKey AccountKey { get; set; } = default!;
    public string Name { get; set; } = default!;
    public EmailAddress Email { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public AccountStatus Status { get; set; } = AccountStatus.Active;
}