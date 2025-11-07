namespace Nubulus.Backend.Api.Features.Account.GetAccount;

public class GetAccountRequest
{
    public const string Route = "/api/v1/accounts/{accountKey}";
    public string AccountKey { get; init; } = string.Empty;
}