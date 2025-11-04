using MongoDB.Driver;
using MongoDB.Driver.Linq;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.DTOs;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Enums;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Requests;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.ValueObjects;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Abstractions;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Accounts.Common;
using NUBULUS.AccountsAppsPortalBackEnd.Domain.Entities;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Accounts.UpdateAccount;

internal sealed class UpdateAccountResponse : IGenericResponse<AccountInfoDTO>
{
    public ResultType ResultType { get; init; }
    public string? Message { get; init; }
    public Dictionary<string, string[]>? ValidationErrors { get; init; }
    public AccountInfoDTO? Data { get; init; }

    private UpdateAccountResponse(ResultType resultType, string? message, Dictionary<string, string[]>? validationErrors, AccountInfoDTO? account)
    {
        ResultType = resultType;
        Message = message;
        ValidationErrors = validationErrors;
        Data = account;
    }

    public static UpdateAccountResponse Success(AccountInfoDTO account) =>
        new(ResultType.Ok, "Account updated successfully", null, account);

    public static UpdateAccountResponse NotFound(string message) =>
        new(ResultType.Error, message, null, null);

    public static UpdateAccountResponse DataExists(string message) =>
        new(ResultType.Conflict, message, null, null);

    public static UpdateAccountResponse Error(string message) =>
        new(ResultType.Error, message, null, null);

    public static UpdateAccountResponse ValidationError(Dictionary<string, string[]> errors) =>
        new(ResultType.Problems, "Validation errors", errors, null);
}

public class UpdateAccountService : IUpdateAccountService
{
    private readonly IAccountsCommandsRepository _accountsCommandsRepository;
    private readonly IAccountsQueriesRepository _accountsQueriesRepository;
    private readonly IUsersCommandsRepository _usersCommandsRepository;
    private readonly IUsersQueriesRepository _usersQueriesRepository;
    private readonly IAccountsUsersQueriesRepository _accountUsersQueriesRepository;

    public UpdateAccountService(
        IAccountsCommandsRepository accountsCommandsRepository,
        IAccountsQueriesRepository accountsQueriesRepository,
        IUsersCommandsRepository usersCommandsRepository,
        IUsersQueriesRepository usersQueriesRepository,
        IAccountsUsersQueriesRepository accountUsersQueriesRepository)
    {
        _accountsCommandsRepository = accountsCommandsRepository;
        _accountsQueriesRepository = accountsQueriesRepository;
        _usersCommandsRepository = usersCommandsRepository;
        _usersQueriesRepository = usersQueriesRepository;
        _accountUsersQueriesRepository = accountUsersQueriesRepository;
    }

    public async Task<IGenericResponse<AccountInfoDTO>> ExecuteAsync(IdObject accountId, UpdateAccountRequest request)
    {
        try
        {
            IdObject.ValidateId(accountId.Value);
            request.Validate();
        }
        catch (Exception ex)
        {
            return UpdateAccountResponse.ValidationError(new Dictionary<string, string[]>
            {
                { "request", new[] { ex.Message } }
            });
        }

        var account = await _accountsQueriesRepository.GetAll()
            .Where(a => a.Id == accountId.Value)
            .FirstOrDefaultAsync();

        if (account is null)
        {
            return UpdateAccountResponse.NotFound($"Account with ID '{accountId.Value}' not found.");
        }

        if (account.Name != request.Name)
        {
            var existAccountName = await _accountsQueriesRepository.GetAll()
                .AnyAsync(a => a.Name == request.Name && a.Id != accountId.Value);

            if (existAccountName)
            {
                return UpdateAccountResponse.DataExists($"An account with the name '{request.Name}' already exists.");
            }
        }

        var accountUser = await _accountUsersQueriesRepository.GetAll()
            .Where(au => au.AccountId == accountId.Value && au.Role == AccountsUsersRole.Owner)
            .FirstOrDefaultAsync();

        if (accountUser is null)
        {
            return UpdateAccountResponse.Error("Owner user not found for this account.");
        }

        var user = await _usersQueriesRepository.GetOneAsync(accountUser.UserId);

        if (user is null)
        {
            return UpdateAccountResponse.Error("Owner user details not found.");
        }

        if (user.Email != request.UserEmail)
        {
            var existUserEmail = await _usersQueriesRepository.GetAll()
                .AnyAsync(u => u.Email == request.UserEmail && u.Id != user.Id);

            if (existUserEmail)
            {
                return UpdateAccountResponse.DataExists($"A user with the email '{request.UserEmail}' already exists.");
            }
        }

        if (user.Phone != request.UserPhone)
        {
            var existUserPhone = await _usersQueriesRepository.GetAll()
                .AnyAsync(u => u.Phone == request.UserPhone && u.Id != user.Id);

            if (existUserPhone)
            {
                return UpdateAccountResponse.DataExists($"A user with the phone number '{request.UserPhone}' already exists.");
            }
        }

        try
        {
            account.Name = request.Name;
            await _accountsCommandsRepository.UpdateAsync(account.Id, account);

            user.Name = request.UserName;
            user.Email = request.UserEmail;
            user.Phone = request.UserPhone;
            await _usersCommandsRepository.UpdateAsync(user.Id, user);

            return UpdateAccountResponse.Success(AccountMappers.ToDTO(account, user));
        }
        catch (Exception ex)
        {
            return UpdateAccountResponse.Error($"An error occurred while updating the account: {ex.Message}");
        }
    }
}
