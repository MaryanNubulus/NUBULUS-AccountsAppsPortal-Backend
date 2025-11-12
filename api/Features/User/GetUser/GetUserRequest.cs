namespace Nubulus.Backend.Api.Features.User;

public class GetUserRequest
{
    public const string Route = "/api/v1/accounts/{accountId}/users/{userId}";
    public int AccountId { get; set; }
    public int UserId { get; set; }
}
