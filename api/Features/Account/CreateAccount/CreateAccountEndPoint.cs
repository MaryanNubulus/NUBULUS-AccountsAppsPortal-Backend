using Nubulus.Backend.Api.Features.Common;

namespace Nubulus.Backend.Api.Features.Account.CreateAccount;

public static class CreateAccountEndPoint
{
    public static WebApplication MapCreateAccountEndPoint(this WebApplication app)
    {
        app.MapPost("/api/v1/accounts", async (CreateAccountRequest request, CreateAccountService service, CancellationToken cancellationToken) =>
        {
            var response = await service.ExecuteAsync(request, cancellationToken);
            return response.ResultType switch
            {
                ResultType.Ok => Results.Created($"/api/v1/accounts/{response.Data}", null),
                ResultType.Conflict => Results.Conflict(response.Message),
                ResultType.Problems => Results.ValidationProblem(response.ValidationErrors!),
                _ => Results.Problem(response.Message)
            };
        })
        .WithName("CreateAccount")
        .WithTags("Accounts");

        return app;
    }
}