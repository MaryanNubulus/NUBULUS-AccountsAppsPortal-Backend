namespace Nubulus.Backend.Api.Features.User.Common;

public class UserDto
{
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool IsCreator { get; set; }  // Indica si és el creador de l'Account
}
