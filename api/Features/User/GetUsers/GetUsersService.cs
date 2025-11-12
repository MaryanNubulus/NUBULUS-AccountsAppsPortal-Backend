using Nubulus.Backend.Api.Features.Common;
using Nubulus.Backend.Api.Features.User.Common;
using Nubulus.Domain.Abstractions;
using Nubulus.Domain.ValueObjects;

namespace Nubulus.Backend.Api.Features.User.GetUsers;

public class GetUsersService
{
    internal class GetUsersResponse : IGenericResponse<PaginatedResponse<UserDto>>
    {
        public ResultType ResultType { get; set; } = ResultType.None;
        public string? Message { get; set; } = null;
        public PaginatedResponse<UserDto>? Data { get; set; } = null;
        public Dictionary<string, string[]>? ValidationErrors { get; set; } = null;

        private GetUsersResponse(ResultType resultType, string? message, PaginatedResponse<UserDto>? data)
        {
            ResultType = resultType;
            Message = message;
            Data = data;
            ValidationErrors = null;
        }

        public static GetUsersResponse Success(PaginatedResponse<UserDto> data)
        {
            return new GetUsersResponse(ResultType.Ok, null, data);
        }

        public static GetUsersResponse NotFound(string message)
        {
            return new GetUsersResponse(ResultType.NotFound, message, null);
        }

        public static GetUsersResponse Error(string message)
        {
            return new GetUsersResponse(ResultType.Error, message, null);
        }
    }

    private readonly IUnitOfWork _unitOfWork;

    public GetUsersService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IGenericResponse<PaginatedResponse<UserDto>>> ExecuteAsync(GetUsersRequest request, CancellationToken cancellationToken)
    {
        try
        {
            // Verificar que el Account existe
            var accountId = new AccountId(request.AccountId);
            var account = await _unitOfWork.Accounts.GetAccountByIdAsync(accountId, cancellationToken);
            if (account == null)
            {
                return GetUsersResponse.NotFound($"Account with ID '{request.AccountId}' not found.");
            }

            var totalCount = await _unitOfWork.Users.CountUsersAsync(accountId, request.SearchTerm, cancellationToken);

            if (totalCount == 0)
            {
                return GetUsersResponse.Success(new PaginatedResponse<UserDto>(
                    totalCount: 0,
                    pageNumber: request.PageNumber ?? 1,
                    pageSize: request.PageSize ?? 10,
                    items: new List<UserDto>()
                ));
            }

            var usersQuery = await _unitOfWork.Users.GetUsersAsync(accountId, request.SearchTerm, request.PageNumber, request.PageSize, cancellationToken);

            var userDtos = usersQuery.ToList().ToDto();

            var paginatedResponse = new PaginatedResponse<UserDto>(
                totalCount: totalCount,
                pageNumber: request.PageNumber ?? 1,
                pageSize: request.PageSize ?? 10,
                items: userDtos
            );

            return GetUsersResponse.Success(paginatedResponse);
        }
        catch (Exception ex)
        {
            return GetUsersResponse.Error($"An error occurred while retrieving users: {ex.Message}");
        }
    }
}
