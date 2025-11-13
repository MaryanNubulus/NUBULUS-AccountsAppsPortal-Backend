using Nubulus.Backend.Api.Features.Common;
using Nubulus.Backend.Api.Features.User.Common;
using Nubulus.Domain.Abstractions;
using Nubulus.Domain.ValueObjects;

namespace Nubulus.Backend.Api.Features.User.GetUsersToShare;

public class GetUsersToShareService
{
    internal class GetUsersToShareResponse : IGenericResponse<PaginatedResponse<UserToShareDto>>
    {
        public ResultType ResultType { get; set; } = ResultType.None;
        public string? Message { get; set; } = null;
        public PaginatedResponse<UserToShareDto>? Data { get; set; } = null;
        public Dictionary<string, string[]>? ValidationErrors { get; set; } = null;

        private GetUsersToShareResponse(ResultType resultType, string? message, PaginatedResponse<UserToShareDto>? data)
        {
            ResultType = resultType;
            Message = message;
            Data = data;
            ValidationErrors = null;
        }

        public static GetUsersToShareResponse Success(PaginatedResponse<UserToShareDto> data)
        {
            return new GetUsersToShareResponse(ResultType.Ok, null, data);
        }

        public static GetUsersToShareResponse NotFound(string message)
        {
            return new GetUsersToShareResponse(ResultType.NotFound, message, null);
        }

        public static GetUsersToShareResponse Error(string message)
        {
            return new GetUsersToShareResponse(ResultType.Error, message, null);
        }

        public static GetUsersToShareResponse ValidationError(Dictionary<string, string[]> errors)
        {
            return new GetUsersToShareResponse(ResultType.Problems, null, null)
            {
                ValidationErrors = errors
            };
        }
    }

    private readonly IUnitOfWork _unitOfWork;

    public GetUsersToShareService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IGenericResponse<PaginatedResponse<UserToShareDto>>> ExecuteAsync(GetUsersToShareRequest request, CancellationToken cancellationToken)
    {
        // Validar request
        try
        {
            request.Validate();
        }
        catch (Exception ex)
        {
            return GetUsersToShareResponse.ValidationError(
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
                return GetUsersToShareResponse.NotFound($"Account with ID '{request.AccountId}' not found.");
            }

            // Obtener total de usuarios disponibles para compartir con filtro
            var totalCount = await _unitOfWork.Users.CountUsersToShareAsync(accountId, request.SearchTerm, cancellationToken);

            if (totalCount == 0)
            {
                return GetUsersToShareResponse.Success(new PaginatedResponse<UserToShareDto>(
                    totalCount: 0,
                    pageNumber: request.PageNumber ?? 1,
                    pageSize: request.PageSize ?? 10,
                    items: new List<UserToShareDto>()
                ));
            }

            // Obtener usuarios disponibles paginados
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;
            var usersQuery = await _unitOfWork.Users.GetUsersToShareAsync(accountId, request.SearchTerm, pageNumber, pageSize, cancellationToken);
            var users = usersQuery.ToList();

            // Mapear a DTO
            var userDtos = users.ToShareDto();

            var paginatedResponse = new PaginatedResponse<UserToShareDto>(
                totalCount: totalCount,
                pageNumber: pageNumber,
                pageSize: pageSize,
                items: userDtos
            );

            return GetUsersToShareResponse.Success(paginatedResponse);
        }
        catch (Exception ex)
        {
            return GetUsersToShareResponse.Error($"Error retrieving users to share: {ex.Message}");
        }
    }
}
