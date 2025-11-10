using Nubulus.Backend.Api.Features.Account.Common;
using Nubulus.Backend.Api.Features.Common;
using Nubulus.Domain.Abstractions;

namespace Nubulus.Backend.Api.Features.Account.GetAccount;

public class GetAccountService
{
    internal class GetAccountResponse : IGenericResponse<AccountInfoDto>
    {
        public ResultType ResultType { get; set; } = ResultType.None;
        public string? Message { get; set; } = null;
        public AccountInfoDto? Data { get; set; } = null;

        public Dictionary<string, string[]>? ValidationErrors { get; set; } = null;

        private GetAccountResponse(ResultType resultType, string? message,
                                    AccountInfoDto? data)
        {
            ResultType = resultType;
            Message = message;
            Data = data;
            ValidationErrors = null;
        }

        public static GetAccountResponse Success(AccountInfoDto data)
        {
            return new GetAccountResponse(ResultType.Ok, null, data);
        }

        public static GetAccountResponse NotFound(string message)
        {
            return new GetAccountResponse(ResultType.NotFound, message, null);
        }

        public static GetAccountResponse Error(string message)
        {
            return new GetAccountResponse(ResultType.Error, message, null);
        }
    }

    private readonly IAccountsRepository _accountsRepository;

    public GetAccountService(IAccountsRepository accountsRepository)
    {
        _accountsRepository = accountsRepository;
    }

    public async Task<IGenericResponse<AccountInfoDto>> ExecuteAsync(GetAccountRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var account = await _accountsRepository.GetAccountByKeyAsync(request.AccountKey, cancellationToken);

            if (account == null)
            {
                return GetAccountResponse.NotFound($"Account with key '{request.AccountKey}' not found.");
            }

            return GetAccountResponse.Success(account.ToInfoDto());
        }
        catch (Exception ex)
        {
            return GetAccountResponse.Error($"An error occurred while retrieving the account: {ex.Message}");
        }
    }
}