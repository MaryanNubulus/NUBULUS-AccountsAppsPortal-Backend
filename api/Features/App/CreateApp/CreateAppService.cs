using Nubulus.Backend.Api.Features.Common;
using Nubulus.Domain.Abstractions;
using Nubulus.Domain.ValueObjects;

namespace Nubulus.Backend.Api.Features.App.CreateApp;

public class CreateAppService
{
    internal class CreateAppResponse : IGenericResponse<int?>
    {
        public ResultType ResultType { get; set; } = ResultType.None;
        public string? Message { get; set; } = null;
        public Dictionary<string, string[]>? ValidationErrors { get; set; } = null;
        public int? Data { get; set; } = null;

        private CreateAppResponse(ResultType resultType, string? message, Dictionary<string, string[]>? validationErrors, int? data)
        {
            ResultType = resultType;
            Message = message;
            ValidationErrors = validationErrors;
            Data = data;
        }

        public static CreateAppResponse Success()
        {
            return new CreateAppResponse(ResultType.Ok, null, null, null);
        }

        public static CreateAppResponse DataExists(string message)
        {
            return new CreateAppResponse(ResultType.Conflict, message, null, null);
        }

        public static CreateAppResponse Error(string message)
        {
            return new CreateAppResponse(ResultType.Error, message, null, null);
        }

        public static CreateAppResponse ValidationError(Dictionary<string, string[]> validationErrors)
        {
            return new CreateAppResponse(ResultType.Problems, "Validation errors occurred.", validationErrors, null);
        }
    }

    private readonly IUnitOfWork _unitOfWork;

    public CreateAppService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IGenericResponse<int?>> ExecuteAsync(CreateAppRequest request, string userContextEmail, CancellationToken cancellationToken)
    {
        if (request.Validate().Count > 0)
        {
            return CreateAppResponse.ValidationError(request.Validate());
        }

        var existingApp = await _unitOfWork.Apps.AppKeyExistsAsync(new AppKey(request.Key), cancellationToken);

        if (existingApp)
        {
            return CreateAppResponse.DataExists("An app with the same Key already exists.");
        }

        try
        {
            var command = new Domain.Entities.App.CreateApp(
                new AppKey(request.Key),
                request.Name
            );

            await _unitOfWork.Apps.CreateAppAsync(command, new EmailAddress(userContextEmail), cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            return CreateAppResponse.Error($"An error occurred while creating the app: {ex.Message}");
        }

        return CreateAppResponse.Success();
    }
}
