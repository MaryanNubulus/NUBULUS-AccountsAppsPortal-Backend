using Nubulus.Backend.Api.Features.App.Common;
using Nubulus.Backend.Api.Features.Common;
using Nubulus.Domain.Abstractions;

namespace Nubulus.Backend.Api.Features.App.GetApps;

public class GetAppsService
{
    internal class GetAppsResponse : IGenericResponse<PaginatedResponse<AppDto>>
    {
        public ResultType ResultType { get; set; } = ResultType.None;
        public string? Message { get; set; } = null;
        public PaginatedResponse<AppDto>? Data { get; set; } = null;
        public Dictionary<string, string[]>? ValidationErrors { get; set; } = null;

        private GetAppsResponse(ResultType resultType, string? message, PaginatedResponse<AppDto>? data)
        {
            ResultType = resultType;
            Message = message;
            Data = data;
            ValidationErrors = null;
        }

        public static GetAppsResponse Success(PaginatedResponse<AppDto> data)
        {
            return new GetAppsResponse(ResultType.Ok, null, data);
        }

        public static GetAppsResponse Error(string message)
        {
            return new GetAppsResponse(ResultType.Error, message, null);
        }
    }

    private readonly IUnitOfWork _unitOfWork;

    public GetAppsService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IGenericResponse<PaginatedResponse<AppDto>>> ExecuteAsync(GetAppsRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var totalCount = await _unitOfWork.Apps.CountAppsAsync(request.SearchTerm, cancellationToken);

            if (totalCount == 0)
            {
                return GetAppsResponse.Success(new PaginatedResponse<AppDto>(
                    totalCount: 0,
                    pageNumber: request.PageNumber ?? 1,
                    pageSize: request.PageSize ?? 10,
                    items: new List<AppDto>()
                ));
            }

            var appsQuery = await _unitOfWork.Apps.GetAppsAsync(request.SearchTerm, request.PageNumber, request.PageSize, cancellationToken);

            var appDtos = appsQuery.ToList().ToDto();

            var paginatedResponse = new PaginatedResponse<AppDto>(
                totalCount: totalCount,
                pageNumber: request.PageNumber ?? 1,
                pageSize: request.PageSize ?? 10,
                items: appDtos
            );

            return GetAppsResponse.Success(paginatedResponse);
        }
        catch (Exception ex)
        {
            return GetAppsResponse.Error($"An error occurred while retrieving apps: {ex.Message}");
        }
    }
}
