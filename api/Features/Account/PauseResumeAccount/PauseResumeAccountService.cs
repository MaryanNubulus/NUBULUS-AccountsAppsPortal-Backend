using Nubulus.Backend.Api.Features.Common;
using Nubulus.Domain.Abstractions;
using Nubulus.Domain.ValueObjects;

namespace Nubulus.Backend.Api.Features.Account.PauseResumeAccount;

public class PauseResumeAccountService
{
    internal class PauseResumeAccountResponse : IGenericResponse<int?>
    {
        public ResultType ResultType { get; set; } = ResultType.None;

        public string? Message { get; set; } = null;

        public Dictionary<string, string[]>? ValidationErrors { get; set; } = null;

        public int? Data { get; set; } = null;

        private PauseResumeAccountResponse(ResultType resultType, string? message,
                                    int? data)
        {
            ResultType = resultType;
            Message = message;
            Data = data;
            ValidationErrors = null;
        }

        public static PauseResumeAccountResponse Success(int data)
        {
            return new PauseResumeAccountResponse(ResultType.Ok, null, data);
        }
        public static PauseResumeAccountResponse NotFound(string message)
        {
            return new PauseResumeAccountResponse(ResultType.NotFound, message, null);
        }
        public static PauseResumeAccountResponse Error(string message)
        {
            return new PauseResumeAccountResponse(ResultType.Error, message, null);
        }
    }

    private readonly IUnitOfWork _unitOfWork;

    public PauseResumeAccountService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IGenericResponse<int?>> ExecuteAsync(PauseResumeAccountRequest request, bool Pause, string currentUserEmail, CancellationToken cancellationToken)
    {
        try
        {
            var account = await _unitOfWork.Accounts.GetAccountByIdAsync(new AccountId(request.AccountId), cancellationToken);

            if (account == null)
            {
                return PauseResumeAccountResponse.NotFound($"Account with ID '{request.AccountId}' not found.");
            }

            if (Pause)
            {
                await _unitOfWork.Accounts.PauseAccountAsync(account.AccountId, new EmailAddress(currentUserEmail), cancellationToken);
                return PauseResumeAccountResponse.Success(request.AccountId);
            }
            else
            {
                await _unitOfWork.Accounts.ResumeAccountAsync(account.AccountId, new EmailAddress(currentUserEmail), cancellationToken);
                return PauseResumeAccountResponse.Success(request.AccountId);
            }
        }
        catch (Exception ex)
        {
            return PauseResumeAccountResponse.Error($"An error occurred while processing the request: {ex.Message}");
        }
    }
}
