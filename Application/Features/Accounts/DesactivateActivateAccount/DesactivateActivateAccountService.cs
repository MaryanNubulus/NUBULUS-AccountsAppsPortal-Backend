using MongoDB.Driver;
using MongoDB.Driver.Linq;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.DTOs;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Enums;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.ValueObjects;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Abstractions;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Accounts.Common;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Accounts.DesactivateActivateAccount;

internal sealed class DesactivateActivateAccountResponse : IGenericResponse<AccountInfoDTO>
{
    public ResultType ResultType { get; init; }
    public string? Message { get; init; }
    public Dictionary<string, string[]>? ValidationErrors { get; init; }
    public AccountInfoDTO? Data { get; init; }

    private DesactivateActivateAccountResponse(ResultType resultType, string? message, Dictionary<string, string[]>? validationErrors, AccountInfoDTO? account)
    {
        ResultType = resultType;
        Message = message;
        ValidationErrors = validationErrors;
        Data = account;
    }

    public static DesactivateActivateAccountResponse Success() =>
        new(ResultType.Ok, "Account status updated successfully", null, null);

    public static DesactivateActivateAccountResponse NotFound(string message) =>
        new(ResultType.Error, message, null, null);

    public static DesactivateActivateAccountResponse Error(string message) =>
        new(ResultType.Error, message, null, null);

    public static DesactivateActivateAccountResponse ValidationError(Dictionary<string, string[]> errors) =>
        new(ResultType.Problems, "Validation errors", errors, null);
}

public class DesactivateActivateAccountService : IDesactivateActivateAccountService
{
    private readonly IAccountsCommandsRepository _accountsCommandsRepository;
    private readonly IAccountsQueriesRepository _accountsQueriesRepository;
    private readonly IAccountsUsersQueriesRepository _accountsUsersQueriesRepository;
    private readonly IUsersQueriesRepository _usersQueriesRepository;
    private readonly IUsersCommandsRepository _usersCommandsRepository;

    public DesactivateActivateAccountService(
        IAccountsCommandsRepository accountsCommandsRepository,
        IAccountsQueriesRepository accountsQueriesRepository,
        IAccountsUsersQueriesRepository accountsUsersQueriesRepository,
        IUsersQueriesRepository usersQueriesRepository,
        IUsersCommandsRepository usersCommandsRepository)
    {
        _accountsCommandsRepository = accountsCommandsRepository;
        _accountsQueriesRepository = accountsQueriesRepository;
        _accountsUsersQueriesRepository = accountsUsersQueriesRepository;
        _usersQueriesRepository = usersQueriesRepository;
        _usersCommandsRepository = usersCommandsRepository;
    }

    public async Task<IGenericResponse<AccountInfoDTO>> ExecuteAsync(IdObject accountId, bool activate)
    {
        try
        {
            IdObject.ValidateId(accountId.Value);
        }
        catch (ArgumentException ex)
        {
            return DesactivateActivateAccountResponse.ValidationError(new Dictionary<string, string[]>
            {
                { "accountId", new[] { ex.Message } }
            });
        }

        try
        {
            var account = await _accountsQueriesRepository.GetAll()
                .Where(a => a.Id == accountId.Value)
                .FirstOrDefaultAsync();

            if (account is null)
            {
                return DesactivateActivateAccountResponse.NotFound($"Account with ID '{accountId.Value}' not found.");
            }

            if (account.IsActive != activate)
            {
                account.IsActive = activate;
                await _accountsCommandsRepository.UpdateAsync(accountId.Value, account);
                var usersIds = await _accountsUsersQueriesRepository.GetAll()
                    .Where(au => au.AccountId == accountId.Value)
                    .Select(au => au.UserId)
                    .ToListAsync();
                var users = await _usersQueriesRepository.GetAll()
                    .Where(u => usersIds.Contains(u.Id))
                    .ToListAsync();
                foreach (var user in users)
                {
                    user.IsActive = activate;
                    await _usersCommandsRepository.UpdateAsync(user.Id, user);
                }
            }

            return DesactivateActivateAccountResponse.Success();
        }
        catch (Exception ex)
        {
            return DesactivateActivateAccountResponse.Error($"An error occurred while updating the account: {ex.Message}");
        }
    }
}
