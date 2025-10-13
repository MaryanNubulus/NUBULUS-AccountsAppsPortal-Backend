namespace NUBULUS.AccountsAppsPortalBackEnd;

public record User(Guid Id, string Username, string Email)
{
    public override string ToString()
    {
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{Id}|{Username}|{Email}"));
    }
}
