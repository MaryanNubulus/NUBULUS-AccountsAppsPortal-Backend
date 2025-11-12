namespace Nubulus.Backend.Api.Features.User;

public class PauseResumeUserRequest
{
    public const string PauseRoute = "/api/v1/accounts/{accountId}/users/{userId}/pause";
    public const string ResumeRoute = "/api/v1/accounts/{accountId}/users/{userId}/resume";
    public int AccountId { get; set; }
    public int UserId { get; set; }
}
