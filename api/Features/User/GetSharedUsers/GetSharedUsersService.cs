using Nubulus.Backend.Api.Features.Common;
using Nubulus.Backend.Api.Features.User.Common;
using Nubulus.Domain.Abstractions;
using Nubulus.Domain.ValueObjects;

namespace Nubulus.Backend.Api.Features.User.GetSharedUsers;

public class GetSharedUsersService
{
    internal class GetSharedUsersResponse : IGenericResponse<PaginatedResponse<UserToShareDto>>
    {
        public ResultType ResultType { get; set; } = ResultType.None;
        public string? Message { get; set; } = null;
        public PaginatedResponse<UserToShareDto>? Data { get; set; } = null;
        public Dictionary<string, string[]>? ValidationErrors { get; set; } = null;

        private GetSharedUsersResponse(ResultType resultType, string? message, PaginatedResponse<UserToShareDto>? data)
        {
            ResultType = resultType;
            Message = message;
            Data = data;
            ValidationErrors = null;
        }

        public static GetSharedUsersResponse Success(PaginatedResponse<UserToShareDto> data)
        {
            return new GetSharedUsersResponse(ResultType.Ok, null, data);
        }

        public static GetSharedUsersResponse NotFound(string message)
        {
            return new GetSharedUsersResponse(ResultType.NotFound, message, null);
        }

        public static GetSharedUsersResponse Error(string message)
        {
            return new GetSharedUsersResponse(ResultType.Error, message, null);
        }

        public static GetSharedUsersResponse ValidationError(Dictionary<string, string[]> errors)
        {
            return new GetSharedUsersResponse(ResultType.Problems, null, null)
            {
                ValidationErrors = errors
            };
        }
    }

    private readonly IUnitOfWork _unitOfWork;

    public GetSharedUsersService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IGenericResponse<PaginatedResponse<UserToShareDto>>> ExecuteAsync(GetSharedUsersRequest request, CancellationToken cancellationToken)
    {
        // Validar request
        try
        {
            request.Validate();
        }
        catch (Exception ex)
        {
            return GetSharedUsersResponse.ValidationError(
                new Dictionary<string, string[]>
                {
                    { "Request", new[] { ex.Message } }
                });
        }

        try
        {
            // Verificar que el Account existe
            var accountId = new AccountId(request.AccountId);
            var account = await _unitOfWork.Accounts.GetAccountByIdAsync(accountId, cancellationToken);
            if (account == null)
            {
                return GetSharedUsersResponse.NotFound($"Account with ID '{request.AccountId}' not found.");
            }

            // Obtener total de usuarios compartidos con filtro
            var totalCount = await _unitOfWork.Users.CountSharedUsersAsync(accountId, request.SearchTerm, cancellationToken);

            if (totalCount == 0)
            {
                return GetSharedUsersResponse.Success(new PaginatedResponse<UserToShareDto>(
                    totalCount: 0,
                    pageNumber: request.PageNumber ?? 1,
                    pageSize: request.PageSize ?? 10,
                    items: new List<UserToShareDto>()
                ));
            }

            // Obtener usuarios compartidos paginados
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;
            var usersQuery = await _unitOfWork.Users.GetSharedUsersAsync(accountId, request.SearchTerm, pageNumber, pageSize, cancellationToken);
            var users = usersQuery.ToList();

            // Mapear a UserToShareDto
            var userDtos = users.ToShareDto();

            var paginatedResponse = new PaginatedResponse<UserToShareDto>(
                totalCount: totalCount,
                pageNumber: pageNumber,
                pageSize: pageSize,
                items: userDtos
            );

            return GetSharedUsersResponse.Success(paginatedResponse);
        }
        catch (Exception ex)
        {
            return GetSharedUsersResponse.Error($"Error retrieving shared users: {ex.Message}");
        }
    }
}
