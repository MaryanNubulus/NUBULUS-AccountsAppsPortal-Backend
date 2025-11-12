namespace Nubulus.Backend.Api.Features.Account.Common;

public class AccountDto
{
    public int AccountId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string NumberId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}
