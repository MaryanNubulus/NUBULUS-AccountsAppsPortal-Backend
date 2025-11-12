namespace Nubulus.Backend.Api.Features.Account.PauseResumeAccount;

public class PauseResumeAccountRequest
{
    public const string RoutePause = "/api/v1/accounts/{accountId}/pause";
    public const string RouteResume = "/api/v1/accounts/{accountId}/resume";
    public int AccountId { get; set; }
}