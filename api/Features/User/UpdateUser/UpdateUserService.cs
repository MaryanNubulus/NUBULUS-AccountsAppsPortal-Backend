using Nubulus.Backend.Api.Features.Common;
using Nubulus.Domain.Abstractions;
using Nubulus.Domain.ValueObjects;

namespace Nubulus.Backend.Api.Features.User.UpdateUser;

public class UpdateUserService
{
    internal class UpdateUserResponse : IGenericResponse<int?>
    {
        public ResultType ResultType { get; set; } = ResultType.None;
        public string? Message { get; set; } = null;
        public Dictionary<string, string[]>? ValidationErrors { get; set; } = null;
        public int? Data { get; set; } = null;

        private UpdateUserResponse(ResultType resultType, string? message, Dictionary<string, string[]>? validationErrors, int? data)
        {
            ResultType = resultType;
            Message = message;
            ValidationErrors = validationErrors;
            Data = data;
        }

        public static UpdateUserResponse Success(int userId)
        {
            return new UpdateUserResponse(ResultType.Ok, null, null, userId);
        }

        public static UpdateUserResponse NotFound(string message)
        {
            return new UpdateUserResponse(ResultType.NotFound, message, null, null);
        }

        public static UpdateUserResponse DataExists(string message)
        {
            return new UpdateUserResponse(ResultType.Conflict, message, null, null);
        }

        public static UpdateUserResponse Error(string message)
        {
            return new UpdateUserResponse(ResultType.Error, message, null, null);
        }

        public static UpdateUserResponse ValidationError(Dictionary<string, string[]> validationErrors)
        {
            return new UpdateUserResponse(ResultType.Problems, "Validation errors occurred.", validationErrors, null);
        }
    }

    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IGenericResponse<int?>> ExecuteAsync(int userId, UpdateUserRequest request, string userContextEmail, CancellationToken cancellationToken)
    {
        if (request.Validate().Count > 0)
        {
            return UpdateUserResponse.ValidationError(request.Validate());
        }

        try
        {
            // Verificar que el Account existe
            var accountId = new AccountId(request.AccountId);
            var account = await _unitOfWork.Accounts.GetAccountByIdAsync(accountId, cancellationToken);
            if (account == null)
            {
                return UpdateUserResponse.NotFound($"Account with ID '{request.AccountId}' not found.");
            }

            var userIdValue = new UserId(userId);

            // Verificar que el User pertenece al Account
            var belongsToAccount = await _unitOfWork.Users.UserBelongsToAccountAsync(userIdValue, accountId, cancellationToken);
            if (!belongsToAccount)
            {
                return UpdateUserResponse.NotFound($"User with ID '{userId}' not found in Account '{request.AccountId}'.");
            }

            var existingUser = await _unitOfWork.Users.GetUserByIdAsync(userIdValue, accountId, cancellationToken);

            if (existingUser == null)
            {
                return UpdateUserResponse.NotFound($"User with ID '{userId}' not found.");
            }

            var duplicateExists = await _unitOfWork.Users.UserInfoExistsAsync(request.FullName, request.Email, cancellationToken, userIdValue);

            if (duplicateExists)
            {
                return UpdateUserResponse.DataExists("A user with the same FullName or Email already exists.");
            }

            var command = new Domain.Entities.User.UpdateUser(
                userIdValue,
                request.FullName,
                new EmailAddress(request.Email),
                request.Phone
            );

            await _unitOfWork.Users.UpdateUserAsync(command, new EmailAddress(userContextEmail), cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return UpdateUserResponse.Success(userId);
        }
        catch (Exception ex)
        {
            return UpdateUserResponse.Error($"An error occurred while updating the user: {ex.Message}");
        }
    }
}
