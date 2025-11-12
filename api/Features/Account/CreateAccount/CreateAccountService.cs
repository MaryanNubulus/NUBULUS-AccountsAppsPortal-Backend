using Nubulus.Backend.Api.Features.Common;
using Nubulus.Domain.Abstractions;
using Nubulus.Domain.ValueObjects;

namespace Nubulus.Backend.Api.Features.Account;

public class CreateAccountService
{
    internal class CreateAccountResponse : IGenericResponse<int?>
    {
        public ResultType ResultType { get; set; } = ResultType.None;

        public string? Message { get; set; } = null;

        public Dictionary<string, string[]>? ValidationErrors { get; set; } = null;

        public int? Data { get; set; } = null;

        private CreateAccountResponse(ResultType resultType, string? message, Dictionary<string, string[]>? validationErrors, int? data)
        {
            ResultType = resultType;
            Message = message;
            ValidationErrors = validationErrors;
            Data = data;
        }
        public static CreateAccountResponse Success()
        {
            return new CreateAccountResponse(ResultType.Ok, null, null, null);
        }
        public static CreateAccountResponse DataExists(string message)
        {
            return new CreateAccountResponse(ResultType.Conflict, message, null, null);
        }
        public static CreateAccountResponse Error(string message)
        {
            return new CreateAccountResponse(ResultType.Error, message, null, null);
        }
        public static CreateAccountResponse ValidationError(Dictionary<string, string[]> validationErrors)
        {
            return new CreateAccountResponse(ResultType.Problems, "Validation errors occurred.", validationErrors, null);
        }
    }


    private readonly IUnitOfWork _unitOfWork;

    public CreateAccountService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IGenericResponse<int?>> ExecuteAsync(CreateAccountRequest request, string userContextEmail, CancellationToken cancellationToken)
    {
        if (request.Validate().Count > 0)
        {
            return CreateAccountResponse.ValidationError(request.Validate());
        }

        var existingAccount = await _unitOfWork.Accounts.AccountInfoExistsAsync(request.Name, request.Email, request.Phone, request.NumberId, cancellationToken);

        if (existingAccount)
        {
            return CreateAccountResponse.DataExists("An account with the same Name, Email, Phone, or NumberId already exists.");
        }
        var accountKey = Guid.NewGuid().ToString();
        var userKey = Guid.NewGuid().ToString();
        var accountUserKey = Guid.NewGuid().ToString();

        try
        {
            var command = new Domain.Entities.Account.CreateAccount(
                new AccountKey(accountKey),
                userKey,
                accountUserKey,
                request.Name,
                request.FullName,
                new EmailAddress(request.Email),
                request.Phone,
                request.Address,
                request.NumberId
            );

            await _unitOfWork.Accounts.CreateAccountAsync(command, new EmailAddress(userContextEmail), cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);

        }
        catch (Exception ex)
        {
            return CreateAccountResponse.Error($"An error occurred while creating the account: {ex.Message}");
        }
        return CreateAccountResponse.Success();
    }
}