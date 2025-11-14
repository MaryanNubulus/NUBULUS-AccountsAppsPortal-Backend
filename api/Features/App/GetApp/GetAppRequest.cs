namespace Nubulus.Backend.Api.Features.App.GetApp;

public class GetAppRequest
{
    public const string Route = "/api/v1/apps/{id}";
    public int Id { get; init; }
}
