namespace NUBULUS.AccountsAppsPortalBackEnd.Domain.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.Empty;
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public static User Empty => new User();
    public static User Create(string email, string name)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            Name = name,
            IsActive = true
        };
    }
    public string EncodeBase64String()
    {
        var data = $"{Email},{Name},{IsActive}";
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(data));
    }
}
