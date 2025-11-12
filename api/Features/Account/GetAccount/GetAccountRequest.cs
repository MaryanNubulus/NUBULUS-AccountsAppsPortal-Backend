namespace Nubulus.Backend.Api.Features.Account.GetAccount;

public class GetAccountRequest
{
    public const string Route = "/api/v1/accounts/{accountId}";
    public int AccountId { get; init; }
}