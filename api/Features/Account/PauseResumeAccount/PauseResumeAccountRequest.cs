namespace Nubulus.Backend.Api.Features.Account.PauseResumeAccount;

public class PauseResumeAccountRequest
{
    public const string RoutePause = "/accounts/{accountId}/pause";
    public const string RouteResume = "/accounts/{accountId}/resume";
    public string AccountKey { get; set; } = string.Empty;
}