namespace Nubulus.Backend.Api.Features.User.GetSharedUsers;

public class GetSharedUsersRequest
{
    public const string Route = "/api/v1/accounts/{accountId}/users/shareds";
    public int AccountId { get; set; }
    public string? SearchTerm { get; set; }
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }

    public GetSharedUsersRequest()
    {
        AccountId = 0;
        SearchTerm = null;
        PageNumber = null;
        PageSize = null;
    }

    public GetSharedUsersRequest Validate()
    {
        if (AccountId <= 0)
            throw new ArgumentException("Account ID must be a positive integer.");

        return this;
    }
}
