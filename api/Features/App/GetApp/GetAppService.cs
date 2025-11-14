using Nubulus.Backend.Api.Features.App.Common;
using Nubulus.Backend.Api.Features.Common;
using Nubulus.Domain.Abstractions;
using Nubulus.Domain.ValueObjects;

namespace Nubulus.Backend.Api.Features.App.GetApp;

public class GetAppService
{
    internal class GetAppResponse : IGenericResponse<AppDto>
    {
        public ResultType ResultType { get; set; } = ResultType.None;
        public string? Message { get; set; } = null;
        public AppDto? Data { get; set; } = null;
        public Dictionary<string, string[]>? ValidationErrors { get; set; } = null;

        private GetAppResponse(ResultType resultType, string? message, AppDto? data)
        {
            ResultType = resultType;
            Message = message;
            Data = data;
            ValidationErrors = null;
        }

        public static GetAppResponse Success(AppDto data)
        {
            return new GetAppResponse(ResultType.Ok, null, data);
        }

        public static GetAppResponse NotFound(string message)
        {
            return new GetAppResponse(ResultType.NotFound, message, null);
        }

        public static GetAppResponse Error(string message)
        {
            return new GetAppResponse(ResultType.Error, message, null);
        }
    }

    private readonly IUnitOfWork _unitOfWork;

    public GetAppService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IGenericResponse<AppDto>> ExecuteAsync(GetAppRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var appEntity = await _unitOfWork.Apps.GetAppByIdAsync(new AppId(request.Id), cancellationToken);

            if (appEntity == null)
            {
                return GetAppResponse.NotFound("App not found.");
            }

            var appDto = appEntity.ToDto();

            return GetAppResponse.Success(appDto);
        }
        catch (Exception ex)
        {
            return GetAppResponse.Error($"An error occurred while retrieving the app: {ex.Message}");
        }
    }
}
