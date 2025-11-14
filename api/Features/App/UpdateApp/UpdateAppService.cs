using Nubulus.Backend.Api.Features.Common;
using Nubulus.Domain.Abstractions;
using Nubulus.Domain.ValueObjects;

namespace Nubulus.Backend.Api.Features.App.UpdateApp;

public class UpdateAppService
{
    internal class UpdateAppResponse : IGenericResponse<int?>
    {
        public ResultType ResultType { get; set; } = ResultType.None;
        public string? Message { get; set; } = null;
        public Dictionary<string, string[]>? ValidationErrors { get; set; } = null;
        public int? Data { get; set; } = null;

        private UpdateAppResponse(ResultType resultType, string? message, Dictionary<string, string[]>? validationErrors, int? data)
        {
            ResultType = resultType;
            Message = message;
            ValidationErrors = validationErrors;
            Data = data;
        }

        public static UpdateAppResponse Success()
        {
            return new UpdateAppResponse(ResultType.Ok, null, null, null);
        }

        public static UpdateAppResponse NotFound(string message)
        {
            return new UpdateAppResponse(ResultType.NotFound, message, null, null);
        }

        public static UpdateAppResponse DataExists(string message)
        {
            return new UpdateAppResponse(ResultType.Conflict, message, null, null);
        }

        public static UpdateAppResponse Error(string message)
        {
            return new UpdateAppResponse(ResultType.Error, message, null, null);
        }

        public static UpdateAppResponse ValidationError(Dictionary<string, string[]> validationErrors)
        {
            return new UpdateAppResponse(ResultType.Problems, "Validation errors occurred.", validationErrors, null);
        }
    }

    private readonly IUnitOfWork _unitOfWork;

    public UpdateAppService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IGenericResponse<int?>> ExecuteAsync(int appId, UpdateAppRequest request, string userContextEmail, CancellationToken cancellationToken)
    {
        if (request.Validate().Count > 0)
        {
            return UpdateAppResponse.ValidationError(request.Validate());
        }

        try
        {
            var existingApp = await _unitOfWork.Apps.GetAppByIdAsync(new AppId(appId), cancellationToken);
            if (existingApp == null)
            {
                return UpdateAppResponse.NotFound("App not found.");
            }

            var command = new Domain.Entities.App.UpdateApp(
                new AppId(appId),
                request.Name
            );

            await _unitOfWork.Apps.UpdateAppAsync(command, new EmailAddress(userContextEmail), cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            return UpdateAppResponse.Error($"An error occurred while updating the app: {ex.Message}");
        }

        return UpdateAppResponse.Success();
    }
}
