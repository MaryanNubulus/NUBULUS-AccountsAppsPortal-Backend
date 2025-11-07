namespace Nubulus.Backend.Api.Features.Account.GetAccounts;

public class GetAccountsRequest
{
    public const string Route = "/api/v1/accounts";
    public int? PageNumber { get; init; }
    public int? PageSize { get; init; }
    public string? SearchTerm { get; init; }
}