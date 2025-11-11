using Nubulus.Backend.Api.Features.Common;
using Nubulus.Domain.Abstractions;
using Nubulus.Domain.ValueObjects;

namespace Nubulus.Backend.Api.Features.Account.PauseResumeAccount;

public class PauseResumeAccountService
{
    internal class PauseResumeAccountResponse : IGenericResponse<string>
    {
        public ResultType ResultType { get; set; } = ResultType.None;

        public string? Message { get; set; } = null;

        public Dictionary<string, string[]>? ValidationErrors { get; set; } = null;

        public string? Data { get; set; } = null;

        private PauseResumeAccountResponse(ResultType resultType, string? message,
                                    string? data)
        {
            ResultType = resultType;
            Message = message;
            Data = data;
            ValidationErrors = null;
        }

        public static PauseResumeAccountResponse Success(string data)
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

    private readonly IAccountsRepository _accountsRepository;

    public PauseResumeAccountService(IAccountsRepository accountsRepository)
    {
        _accountsRepository = accountsRepository;
    }

    public async Task<IGenericResponse<string>> ExecuteAsync(PauseResumeAccountRequest request, bool Pause, string currentUserEmail, CancellationToken cancellationToken)
    {
        try
        {
            var account = await _accountsRepository.GetAccountByKeyAsync(request.AccountKey, cancellationToken);

            if (account == null)
            {
                return PauseResumeAccountResponse.NotFound($"Account with key '{request.AccountKey}' not found.");
            }

            if (Pause)
            {
                await _accountsRepository.PauseAccountAsync(new AccountId(account.Id), new EmailAddress(currentUserEmail), cancellationToken);
                return PauseResumeAccountResponse.Success($"Account with key '{request.AccountKey}' has been paused.");
            }
            else
            {
                await _accountsRepository.ResumeAccountAsync(new AccountId(account.Id), new EmailAddress(currentUserEmail), cancellationToken);
                return PauseResumeAccountResponse.Success($"Account with key '{request.AccountKey}' has been resumed.");
            }
        }
        catch (Exception ex)
        {
            return PauseResumeAccountResponse.Error($"An error occurred while processing the request: {ex.Message}");
        }
    }
}
