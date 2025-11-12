using Microsoft.AspNetCore.Mvc;
using Nubulus.Backend.Api.Features.Common;

namespace Nubulus.Backend.Api.Features.Account.PauseResumeAccount;

public static class PauseResumeAccountEndPoint
{
    public static WebApplication MapPauseResumeAccountEndPoint(this WebApplication app)
    {
        app.MapPatch(PauseResumeAccountRequest.RoutePause, async (HttpContext httpContext, [AsParameters] PauseResumeAccountRequest request, [FromServices] PauseResumeAccountService service, CancellationToken cancellationToken) =>
        {
            var response = await service.ExecuteAsync(request, true, httpContext.User.Identity!.Name!, cancellationToken);
            return response.ResultType switch
            {
                ResultType.Ok => Results.Ok(response.Data),
                ResultType.NotFound => Results.NotFound(response.Message),
                _ => Results.Problem(response.Message)
            };
        })
        .WithName("PauseAccount")
        .WithTags("Accounts")
        .RequireAuthorization();

        app.MapPatch(PauseResumeAccountRequest.RouteResume, async (HttpContext httpContext, [AsParameters] PauseResumeAccountRequest request, [FromServices] PauseResumeAccountService service, CancellationToken cancellationToken) =>
        {
            var response = await service.ExecuteAsync(request, false, httpContext.User.Identity!.Name!, cancellationToken);
            return response.ResultType switch
            {
                ResultType.Ok => Results.Ok(response.Data),
                ResultType.NotFound => Results.NotFound(response.Message),
                _ => Results.Problem(response.Message)
            };
        })
        .WithName("ResumeAccount")
        .WithTags("Accounts")
        .RequireAuthorization();

        return app;
    }
}
