using MongoDB.Driver.Linq;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.DTOs;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Enums;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Requests;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Abstractions;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Accounts.Common;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Accounts.CreateAccount;

internal sealed class CreateAccountResponse : IGenericResponse<AccountInfoDTO>
{
    public ResultType ResultType { get; init; }
    public string? Message { get; init; }
    public Dictionary<string, string[]>? ValidationErrors { get; init; }
    public AccountInfoDTO? Data { get; init; }

    private CreateAccountResponse(ResultType resultType, string? message, Dictionary<string, string[]>? validationErrors, AccountInfoDTO? account)
    {
        ResultType = resultType;
        Message = message;
        ValidationErrors = validationErrors;
        Data = account;
    }

    public static CreateAccountResponse Success() =>
        new(ResultType.Ok, "Account created successfully", null, null);

    public static CreateAccountResponse DataExists(string message) =>
        new(ResultType.Conflict, message, null, null);

    public static CreateAccountResponse Error(string message) =>
        new(ResultType.Error, message, null, null);

    public static CreateAccountResponse ValidationError(Dictionary<string, string[]> errors) =>
        new(ResultType.Problems, "Validation errors", errors, null);
}

public class CreateAccountService : ICreateAccountService
{
    private readonly IAccountsCommandsRepository _accountsCommandsRepository;
    private readonly IAccountsQueriesRepository _accountsQueriesRepository;

    private readonly IUsersCommandsRepository _usersCommandsRepository;
    private readonly IUsersQueriesRepository _usersQueriesRepository;

    private readonly IAccountsUsersCommandsRepository _accountUsersCommandsRepository;
    private readonly IAccountsUsersQueriesRepository _accountUsersQueriesRepository;

    public CreateAccountService(
        IAccountsCommandsRepository accountsCommandsRepository,
        IAccountsQueriesRepository accountsQueriesRepository,
        IUsersCommandsRepository usersCommandsRepository,
        IUsersQueriesRepository usersQueriesRepository,
        IAccountsUsersCommandsRepository accountUsersCommandsRepository,
        IAccountsUsersQueriesRepository accountUsersQueriesRepository)
    {
        _accountsCommandsRepository = accountsCommandsRepository;
        _accountsQueriesRepository = accountsQueriesRepository;
        _usersCommandsRepository = usersCommandsRepository;
        _usersQueriesRepository = usersQueriesRepository;
        _accountUsersCommandsRepository = accountUsersCommandsRepository;
        _accountUsersQueriesRepository = accountUsersQueriesRepository;
    }

    public async Task<IGenericResponse<AccountInfoDTO>> ExecuteAsync(CreateAccountRequest request)
    {
        try
        {
            request.Validate();
        }
        catch (Exception ex)
        {
            return CreateAccountResponse.ValidationError(new Dictionary<string, string[]>
           {
               { "request", new[] { ex.Message } }
           });
        }

        var existAccountName = await _accountsQueriesRepository.GetAll().AnyAsync(a => a.Name == request.AccountName);
        if (existAccountName)
        {
            return CreateAccountResponse.DataExists($"An account with the name '{request.AccountName}' already exists.");
        }

        var existUserEmail = await _usersQueriesRepository.GetAll().AnyAsync(a => a.Email == request.UserEmail);
        if (existUserEmail)
        {
            return CreateAccountResponse.DataExists($"A user with the email '{request.UserEmail}' already exists.");
        }

        var existUserPhone = await _usersQueriesRepository.GetAll().AnyAsync(a => a.Phone == request.UserPhone);
        if (existUserPhone)
        {
            return CreateAccountResponse.DataExists($"A user with the phone number '{request.UserPhone}' already exists.");
        }

        try
        {
            var (account, user, accountsUsers) = request.ToEntities();

            await _accountsCommandsRepository.AddAsync(account);
            await _usersCommandsRepository.AddAsync(user);
            await _accountUsersCommandsRepository.AddAsync(accountsUsers);

            return CreateAccountResponse.Success();
        }
        catch (Exception ex)
        {
            return CreateAccountResponse.Error($"An error occurred while creating the account: {ex.Message}");
        }
    }
}