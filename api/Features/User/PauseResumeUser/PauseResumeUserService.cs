using Nubulus.Backend.Api.Features.Common;
using Nubulus.Domain.Abstractions;
using Nubulus.Domain.ValueObjects;

namespace Nubulus.Backend.Api.Features.User.PauseResumeUser;

public class PauseResumeUserService
{
    internal class PauseResumeUserResponse : IGenericResponse<int?>
    {
        public ResultType ResultType { get; set; } = ResultType.None;
        public string? Message { get; set; } = null;
        public Dictionary<string, string[]>? ValidationErrors { get; set; } = null;
        public int? Data { get; set; } = null;

        private PauseResumeUserResponse(ResultType resultType, string? message, int? data)
        {
            ResultType = resultType;
            Message = message;
            Data = data;
            ValidationErrors = null;
        }

        public static PauseResumeUserResponse Success(int userId)
        {
            return new PauseResumeUserResponse(ResultType.Ok, null, userId);
        }

        public static PauseResumeUserResponse NotFound(string message)
        {
            return new PauseResumeUserResponse(ResultType.NotFound, message, null);
        }

        public static PauseResumeUserResponse Error(string message)
        {
            return new PauseResumeUserResponse(ResultType.Error, message, null);
        }
    }

    private readonly IUnitOfWork _unitOfWork;

    public PauseResumeUserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IGenericResponse<int?>> PauseAsync(int accountId, int userId, string userContextEmail, CancellationToken cancellationToken)
    {
        try
        {
            // Verificar que el Account existe
            var accountIdValue = new AccountId(accountId);
            var account = await _unitOfWork.Accounts.GetAccountByIdAsync(accountIdValue, cancellationToken);
            if (account == null)
            {
                return PauseResumeUserResponse.NotFound($"Account with ID '{accountId}' not found.");
            }

            var userIdValue = new UserId(userId);

            // Verificar que el User pertenece al Account
            var belongsToAccount = await _unitOfWork.Users.UserBelongsToAccountAsync(userIdValue, accountIdValue, cancellationToken);
            if (!belongsToAccount)
            {
                return PauseResumeUserResponse.NotFound($"User with ID '{userId}' not found in Account '{accountId}'.");
            }

            var existingUser = await _unitOfWork.Users.GetUserByIdAsync(userIdValue, accountIdValue, cancellationToken);

            if (existingUser == null)
            {
                return PauseResumeUserResponse.NotFound($"User with ID '{userId}' not found.");
            }

            await _unitOfWork.Users.PauseUserAsync(userIdValue, accountIdValue, new EmailAddress(userContextEmail), cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return PauseResumeUserResponse.Success(userId);
        }
        catch (Exception ex)
        {
            return PauseResumeUserResponse.Error($"An error occurred while pausing the user: {ex.Message}");
        }
    }

    public async Task<IGenericResponse<int?>> ResumeAsync(int accountId, int userId, string userContextEmail, CancellationToken cancellationToken)
    {
        try
        {
            // Verificar que el Account existe
            var accountIdValue = new AccountId(accountId);
            var account = await _unitOfWork.Accounts.GetAccountByIdAsync(accountIdValue, cancellationToken);
            if (account == null)
            {
                return PauseResumeUserResponse.NotFound($"Account with ID '{accountId}' not found.");
            }

            var userIdValue = new UserId(userId);

            // Verificar que el User pertenece al Account
            var belongsToAccount = await _unitOfWork.Users.UserBelongsToAccountAsync(userIdValue, accountIdValue, cancellationToken);
            if (!belongsToAccount)
            {
                return PauseResumeUserResponse.NotFound($"User with ID '{userId}' not found in Account '{accountId}'.");
            }

            var existingUser = await _unitOfWork.Users.GetUserByIdAsync(userIdValue, accountIdValue, cancellationToken);

            if (existingUser == null)
            {
                return PauseResumeUserResponse.NotFound($"User with ID '{userId}' not found.");
            }

            await _unitOfWork.Users.ResumeUserAsync(userIdValue, accountIdValue, new EmailAddress(userContextEmail), cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return PauseResumeUserResponse.Success(userId);
        }
        catch (Exception ex)
        {
            return PauseResumeUserResponse.Error($"An error occurred while resuming the user: {ex.Message}");
        }
    }
}
