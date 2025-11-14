using Nubulus.Backend.Api.Features.Common;
using Nubulus.Domain.Abstractions;
using Nubulus.Domain.ValueObjects;

namespace Nubulus.Backend.Api.Features.App.PauseResumeApp;

public class PauseResumeAppService
{
    internal class PauseResumeAppResponse : IGenericResponse<int?>
    {
        public ResultType ResultType { get; set; } = ResultType.None;
        public string? Message { get; set; } = null;
        public Dictionary<string, string[]>? ValidationErrors { get; set; } = null;
        public int? Data { get; set; } = null;

        private PauseResumeAppResponse(ResultType resultType, string? message, Dictionary<string, string[]>? validationErrors, int? data)
        {
            ResultType = resultType;
            Message = message;
            ValidationErrors = validationErrors;
            Data = data;
        }

        public static PauseResumeAppResponse Success()
        {
            return new PauseResumeAppResponse(ResultType.Ok, null, null, null);
        }

        public static PauseResumeAppResponse NotFound(string message)
        {
            return new PauseResumeAppResponse(ResultType.NotFound, message, null, null);
        }

        public static PauseResumeAppResponse Error(string message)
        {
            return new PauseResumeAppResponse(ResultType.Error, message, null, null);
        }
    }

    private readonly IUnitOfWork _unitOfWork;

    public PauseResumeAppService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IGenericResponse<int?>> PauseAsync(int appId, string userContextEmail, CancellationToken cancellationToken)
    {
        try
        {
            var existingApp = await _unitOfWork.Apps.GetAppByIdAsync(new AppId(appId), cancellationToken);
            if (existingApp == null)
            {
                return PauseResumeAppResponse.NotFound("App not found.");
            }

            await _unitOfWork.Apps.PauseAppAsync(new AppId(appId), new EmailAddress(userContextEmail), cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return PauseResumeAppResponse.Success();
        }
        catch (Exception ex)
        {
            return PauseResumeAppResponse.Error($"An error occurred while pausing the app: {ex.Message}");
        }
    }

    public async Task<IGenericResponse<int?>> ResumeAsync(int appId, string userContextEmail, CancellationToken cancellationToken)
    {
        try
        {
            var existingApp = await _unitOfWork.Apps.GetAppByIdAsync(new AppId(appId), cancellationToken);
            if (existingApp == null)
            {
                return PauseResumeAppResponse.NotFound("App not found.");
            }

            await _unitOfWork.Apps.ResumeAppAsync(new AppId(appId), new EmailAddress(userContextEmail), cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return PauseResumeAppResponse.Success();
        }
        catch (Exception ex)
        {
            return PauseResumeAppResponse.Error($"An error occurred while resuming the app: {ex.Message}");
        }
    }
}
