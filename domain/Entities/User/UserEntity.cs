using Nubulus.Domain.ValueObjects;

namespace Nubulus.Domain.Entities.User;

public class UserEntity
{
    public UserId UserId { get; set; } = default!;
    public UserKey UserKey { get; set; } = default!;
    public string Name { get; set; } = default!;
    public EmailAddress Email { get; set; } = default!;
    public Status Status { get; set; } = Status.Active;
    public bool IsCreator { get; set; } = false;  // Indica si és el creador de l'Account
}
