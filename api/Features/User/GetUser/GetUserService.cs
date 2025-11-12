using Nubulus.Backend.Api.Features.Common;
using Nubulus.Backend.Api.Features.User.Common;
using Nubulus.Domain.Abstractions;
using Nubulus.Domain.ValueObjects;

namespace Nubulus.Backend.Api.Features.User.GetUser;

public class GetUserService
{
    internal class GetUserResponse : IGenericResponse<UserInfoDto>
    {
        public ResultType ResultType { get; set; } = ResultType.None;
        public string? Message { get; set; } = null;
        public UserInfoDto? Data { get; set; } = null;
        public Dictionary<string, string[]>? ValidationErrors { get; set; } = null;

        private GetUserResponse(ResultType resultType, string? message, UserInfoDto? data)
        {
            ResultType = resultType;
            Message = message;
            Data = data;
            ValidationErrors = null;
        }

        public static GetUserResponse Success(UserInfoDto data)
        {
            return new GetUserResponse(ResultType.Ok, null, data);
        }

        public static GetUserResponse NotFound(string message)
        {
            return new GetUserResponse(ResultType.NotFound, message, null);
        }

        public static GetUserResponse Error(string message)
        {
            return new GetUserResponse(ResultType.Error, message, null);
        }
    }

    private readonly IUnitOfWork _unitOfWork;

    public GetUserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IGenericResponse<UserInfoDto>> ExecuteAsync(GetUserRequest request, CancellationToken cancellationToken)
    {
        try
        {
            // Verificar que el Account existe
            var accountId = new AccountId(request.AccountId);
            var account = await _unitOfWork.Accounts.GetAccountByIdAsync(accountId, cancellationToken);
            if (account == null)
            {
                return GetUserResponse.NotFound($"Account with ID '{request.AccountId}' not found.");
            }

            var userId = new UserId(request.UserId);

            // Verificar que el User pertenece al Account
            var belongsToAccount = await _unitOfWork.Users.UserBelongsToAccountAsync(userId, accountId, cancellationToken);
            if (!belongsToAccount)
            {
                return GetUserResponse.NotFound($"User with ID '{request.UserId}' not found in Account '{request.AccountId}'.");
            }

            var user = await _unitOfWork.Users.GetUserByIdAsync(userId, accountId, cancellationToken);

            if (user == null)
            {
                return GetUserResponse.NotFound($"User with ID '{request.UserId}' not found.");
            }

            var userInfoDto = user.ToInfoDto();

            return GetUserResponse.Success(userInfoDto);
        }
        catch (Exception ex)
        {
            return GetUserResponse.Error($"An error occurred while retrieving the user: {ex.Message}");
        }
    }
}
