namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Requests;

public class CreateAccountRequest
{
    public string AccountName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;

    public string UserEmail { get; set; } = string.Empty;

    public string UserPhone { get; set; } = string.Empty;
}