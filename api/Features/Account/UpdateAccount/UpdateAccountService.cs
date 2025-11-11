using Nubulus.Backend.Api.Features.Common;
using Nubulus.Domain.Abstractions;
using Nubulus.Domain.ValueObjects;

namespace Nubulus.Backend.Api.Features.Account.UpdateAccount;

public class UpdateAccountService
{
    internal class UpdateAccountResponse : IGenericResponse<string>
    {
        public ResultType ResultType { get; set; } = ResultType.None;

        public string? Message { get; set; } = null;

        public Dictionary<string, string[]>? ValidationErrors { get; set; } = null;

        public string? Data { get; set; } = null;

        private UpdateAccountResponse(ResultType resultType, string? message, Dictionary<string, string[]>? validationErrors, string? data)
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

    private readonly IAccountsRepository _accountsRepository;
    public UpdateAccountService(IAccountsRepository accountsRepository)
    {
        _accountsRepository = accountsRepository;
    }

    public async Task<IGenericResponse<string>> ExecuteAsync(string accountKey, UpdateAccountRequest request, string currentUserEmail, CancellationToken cancellationToken)
    {
        if (request.Validate().Count > 0)
        {
            return UpdateAccountResponse.ValidationError(request.Validate());
        }

        var account = await _accountsRepository.GetAccountByKeyAsync(accountKey, cancellationToken);
        if (account == null)
        {
            return UpdateAccountResponse.NotFound($"Account with key {accountKey} not found.");
        }

        var existingAccount = await _accountsRepository.AccountInfoExistsAsync(request.Name, request.Email, request.Phone, request.NumberId, cancellationToken, account.Id);
        if (existingAccount)
        {
            return UpdateAccountResponse.DataExists("An account with the provided Name, Email, Phone, or NumberId already exists.");
        }

        try
        {
            var command = new Domain.Entities.Account.UpdateAccount(
                account.Id,
                request.Name,
                new EmailAddress(request.Email),
                request.Phone,
                request.Address,
                request.NumberId
            );

            await _accountsRepository.UpdateAccountAsync(command, new EmailAddress(currentUserEmail), cancellationToken);
        }
        catch (Exception ex)
        {
            return UpdateAccountResponse.Error($"An error occurred while updating the account: {ex.Message}");
        }

        return UpdateAccountResponse.Success();
    }
}