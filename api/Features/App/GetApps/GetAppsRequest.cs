namespace Nubulus.Backend.Api.Features.App.GetApps;

public class GetAppsRequest
{
    public const string Route = "/api/v1/apps";
    public int? PageNumber { get; init; }
    public int? PageSize { get; init; }
    public string? SearchTerm { get; init; }
}
