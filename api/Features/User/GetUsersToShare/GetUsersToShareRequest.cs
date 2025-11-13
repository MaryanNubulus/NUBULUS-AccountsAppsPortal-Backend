namespace Nubulus.Backend.Api.Features.User.GetUsersToShare;

public class GetUsersToShareRequest
{
    public const string Route = "/api/v1/accounts/{accountId}/users/to-share";

    public int AccountId { get; init; }
    public string? SearchTerm { get; init; }
    public int? PageNumber { get; init; }
    public int? PageSize { get; init; }

    public GetUsersToShareRequest()
    {
        AccountId = 0;
        SearchTerm = null;
        PageNumber = null;
        PageSize = null;
    }

    private GetUsersToShareRequest(int accountId, string? searchTerm, int? pageNumber, int? pageSize)
    {
        AccountId = accountId;
        SearchTerm = searchTerm;
        PageNumber = pageNumber;
        PageSize = pageSize;
        Validate();
    }

    public GetUsersToShareRequest Validate()
    {
        if (AccountId <= 0)
            throw new ArgumentException("Account ID must be a positive integer.");

        if (PageNumber.HasValue && PageNumber <= 0)
            throw new ArgumentException("Page number must be a positive integer.");

        if (PageSize.HasValue && PageSize <= 0)
            throw new ArgumentException("Page size must be a positive integer.");

        return this;
    }

    public static GetUsersToShareRequest Create(int accountId, string? searchTerm = null, int? pageNumber = null, int? pageSize = null)
    {
        return new GetUsersToShareRequest(accountId, searchTerm, pageNumber, pageSize);
    }
}
