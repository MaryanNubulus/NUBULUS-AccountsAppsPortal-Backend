using Nubulus.Backend.Api.Features.Common;
using Nubulus.Domain.Abstractions;
using Nubulus.Domain.ValueObjects;

namespace Nubulus.Backend.Api.Features.Account.UpdateAccount;

public class UpdateAccountService
{
    internal class UpdateAccountResponse : IGenericResponse<int?>
    {
        public ResultType ResultType { get; set; } = ResultType.None;

        public string? Message { get; set; } = null;

        public Dictionary<string, string[]>? ValidationErrors { get; set; } = null;

        public int? Data { get; set; } = null;

        private UpdateAccountResponse(ResultType resultType, string? message, Dictionary<string, string[]>? validationErrors, int? data)
        {
            ResultType = resultType;
            Message = message;
            ValidationErrors = validationErrors;
            Data = data;
        }
        public static UpdateAccountResponse Success()
        {
            return new UpdateAccountResponse(ResultType.Ok, null, null, null);
        }
        public static UpdateAccountResponse NotFound(string message)
        {
            return new UpdateAccountResponse(ResultType.NotFound, message, null, null);
        }
        public static UpdateAccountResponse DataExists(string message)
        {
            return new UpdateAccountResponse(ResultType.Conflict, message, null, null);
        }
        public static UpdateAccountResponse Error(string message)
        {
            return new UpdateAccountResponse(ResultType.Error, message, null, null);
        }
        public static UpdateAccountResponse ValidationError(Dictionary<string, string[]> validationErrors)
        {
            return new UpdateAccountResponse(ResultType.Problems, "Validation errors occurred.", validationErrors, null);
        }
    }

    private readonly IUnitOfWork _unitOfWork;
    public UpdateAccountService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IGenericResponse<int?>> ExecuteAsync(int accountId, UpdateAccountRequest request, string currentUserEmail, CancellationToken cancellationToken)
    {
        if (request.Validate().Count > 0)
        {
            return UpdateAccountResponse.ValidationError(request.Validate());
        }

        var account = await _unitOfWork.Accounts.GetAccountByIdAsync(new AccountId(accountId), cancellationToken);
        if (account == null)
        {
            return UpdateAccountResponse.NotFound($"Account with ID {accountId} not found.");
        }

        var existingAccount = await _unitOfWork.Accounts.AccountInfoExistsAsync(request.Name, request.Email, request.Phone, request.NumberId, cancellationToken, account.AccountId);
        if (existingAccount)
        {
            return UpdateAccountResponse.DataExists("An account with the provided Name, Email, Phone, or NumberId already exists.");
        }

        try
        {
            var command = new Domain.Entities.Account.UpdateAccount(
                account.AccountId,
                request.Name,
                new EmailAddress(request.Email),
                request.Phone,
                request.Address,
                request.NumberId
            );

            await _unitOfWork.Accounts.UpdateAccountAsync(command, new EmailAddress(currentUserEmail), cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            return UpdateAccountResponse.Error($"An error occurred while updating the account: {ex.Message}");
        }

        return UpdateAccountResponse.Success();
    }
}