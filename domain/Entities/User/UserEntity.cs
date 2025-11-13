using Nubulus.Domain.ValueObjects;

namespace Nubulus.Domain.Entities.User;

public class UserEntity
{
    public UserId UserId { get; set; } = default!;
    public UserKey UserKey { get; set; } = default!;
    public AccountKey ParentKey { get; set; } = default!;  // Compte on s'ha creat l'usuari
    public string FullName { get; set; } = default!;
    public EmailAddress Email { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public string? Password { get; set; }
    public Status Status { get; set; } = Status.Active;
    public bool IsCreator { get; set; } = false;
}
