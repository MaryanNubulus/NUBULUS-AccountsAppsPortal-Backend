using Nubulus.Domain.ValueObjects;

namespace Nubulus.Domain.Entities.Account;

public class AccountEntity
{
    public int Id { get; set; }
    public AccountKey AccountKey { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public EmailAddress Email { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public string Address { get; set; } = default!;
    public string NumberId { get; set; } = string.Empty;
    public Status Status { get; set; } = Status.Active;
}