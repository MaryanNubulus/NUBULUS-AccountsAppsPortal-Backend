namespace Nubulus.Backend.Api.Features.User.Common;

public class UserDto
{
    public int UserId { get; set; }
    public string UserKey { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool IsCreator { get; set; }
}
