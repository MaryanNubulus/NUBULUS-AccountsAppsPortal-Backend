namespace Nubulus.Backend.Api.Features.User.ShareUnshareUser;

public class ShareUnshareUserRequest
{
    public const string ShareRoute = "/api/v1/accounts/{accountId}/users/{userId}/share";
    public const string UnshareRoute = "/api/v1/accounts/{accountId}/users/{userId}/unshare";
    public int AccountId { get; set; }
    public int UserId { get; set; }
}
