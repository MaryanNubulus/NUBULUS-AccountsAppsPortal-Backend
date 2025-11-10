namespace Nubulus.Domain.Entities.UserContext;

public class CurrentUserInfo
{
    public string UserEmail { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public List<string>? Roles { get; set; } = new List<string>();
}