namespace Nubulus.Backend.Api.Features.User;

public class GetUsersRequest
{
    public const string Route = "/api/v1/accounts/{accountId}/users";
    public int AccountId { get; set; }
    public string? SearchTerm { get; set; }
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }
}
