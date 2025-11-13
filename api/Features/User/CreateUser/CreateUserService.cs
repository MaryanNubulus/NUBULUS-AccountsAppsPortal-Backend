using Nubulus.Backend.Api.Features.Common;
using Nubulus.Domain.Abstractions;
using Nubulus.Domain.ValueObjects;

namespace Nubulus.Backend.Api.Features.User;

public class CreateUserService
{
    internal class CreateUserResponse : IGenericResponse<int?>
    {
        public ResultType ResultType { get; set; } = ResultType.None;
        public string? Message { get; set; } = null;
        public Dictionary<string, string[]>? ValidationErrors { get; set; } = null;
        public int? Data { get; set; } = null;

        private CreateUserResponse(ResultType resultType, string? message, Dictionary<string, string[]>? validationErrors, int? data)
        {
            ResultType = resultType;
            Message = message;
            ValidationErrors = validationErrors;
            Data = data;
        }

        public static CreateUserResponse Success()
        {
            return new CreateUserResponse(ResultType.Ok, null, null, null);
        }

        public static CreateUserResponse DataExists(string message)
        {
            return new CreateUserResponse(ResultType.Conflict, message, null, null);
        }

        public static CreateUserResponse NotFound(string message)
        {
            return new CreateUserResponse(ResultType.NotFound, message, null, null);
        }

        public static CreateUserResponse Error(string message)
        {
            return new CreateUserResponse(ResultType.Error, message, null, null);
        }

        public static CreateUserResponse ValidationError(Dictionary<string, string[]> validationErrors)
        {
            return new CreateUserResponse(ResultType.Problems, "Validation errors occurred.", validationErrors, null);
        }
    }

    private readonly IUnitOfWork _unitOfWork;

    public CreateUserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IGenericResponse<int?>> ExecuteAsync(CreateUserRequest request, string userContextEmail, CancellationToken cancellationToken)
    {
        if (request.Validate().Count > 0)
        {
            return CreateUserResponse.ValidationError(request.Validate());
        }

        // Verificar que el Account existe
        var accountId = new AccountId(request.AccountId);
        var account = await _unitOfWork.Accounts.GetAccountByIdAsync(accountId, cancellationToken);
        if (account == null)
        {
            return CreateUserResponse.NotFound($"Account with ID '{request.AccountId}' not found.");
        }

        var existingUser = await _unitOfWork.Users.UserInfoExistsAsync(request.FullName, request.Email, cancellationToken);

        if (existingUser)
        {
            return CreateUserResponse.DataExists("A user with the same FullName or Email already exists.");
        }

        var userKey = Guid.NewGuid().ToString();

        try
        {
            var command = new Domain.Entities.User.CreateUser(
                new UserKey(userKey),
                account.AccountKey,
                request.FullName,
                new EmailAddress(request.Email),
                request.Phone,
                request.Password
            );

            await _unitOfWork.Users.CreateUserAsync(command, accountId, new EmailAddress(userContextEmail), cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            return CreateUserResponse.Error($"An error occurred while creating the user: {ex.Message}");
        }

        return CreateUserResponse.Success();
    }
}
