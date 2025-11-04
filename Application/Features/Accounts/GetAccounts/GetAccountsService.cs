using MongoDB.Driver;
using MongoDB.Driver.Linq;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.DTOs;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Enums;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Abstractions;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Accounts.Common;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Accounts.CreateAccount;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Accounts.GetAccounts;

internal sealed class GetAccountsResponse : IGenericResponse<IEnumerable<AccountInfoDTO>>
{
    public ResultType ResultType { get; init; }
    public string? Message { get; init; }
    public Dictionary<string, string[]>? ValidationErrors { get; init; }
    public IEnumerable<AccountInfoDTO>? Data { get; init; }

    private GetAccountsResponse(ResultType resultType, string? message, Dictionary<string, string[]>? validationErrors, IEnumerable<AccountInfoDTO>? data)
    {
        ResultType = resultType;
        Message = message;
        ValidationErrors = validationErrors;
        Data = data;
    }

    public static GetAccountsResponse Success(IEnumerable<AccountInfoDTO> accounts) =>
        new(ResultType.Ok, null, null, accounts);

    public static GetAccountsResponse Error(string message) =>
        new(ResultType.Error, message, null, null);
}

public class GetAccountsService : IGetAccountsService
{
    private readonly IAccountsQueriesRepository _accountsQueriesRepository;
    private readonly IUsersQueriesRepository _usersQueriesRepository;
    private readonly IAccountsUsersQueriesRepository _accountsUsersQueriesRepository;

    public GetAccountsService(IAccountsQueriesRepository accountsQueriesRepository, IUsersQueriesRepository usersQueriesRepository, IAccountsUsersQueriesRepository accountsUsersQueriesRepository)
    {
        _accountsQueriesRepository = accountsQueriesRepository;
        _usersQueriesRepository = usersQueriesRepository;
        _accountsUsersQueriesRepository = accountsUsersQueriesRepository;
    }

    public async Task<IGenericResponse<IEnumerable<AccountInfoDTO>>> ExecuteAsync()
    {
        try
        {
            var accounts = await _accountsQueriesRepository.GetAll().ToListAsync();
            var users = await _usersQueriesRepository.GetAll().ToListAsync();
            var accountsUsers = await _accountsUsersQueriesRepository.GetAll().ToListAsync();

            var accountsDTO = AccountMappers.ToDTOList(accounts, users, accountsUsers);

            return GetAccountsResponse.Success(accountsDTO);
        }
        catch (Exception ex)
        {
            return GetAccountsResponse.Error($"An error occurred while retrieving accounts: {ex.Message}");
        }
    }
}
