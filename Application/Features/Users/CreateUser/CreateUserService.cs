using MongoDB.Driver.Linq;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.DTOs;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Enums;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Requests;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Abstractions;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Users.Common;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Users.CreateUser;

internal sealed class CreateUserResponse : IGenericResponse<UserInfoDTO>
{
    public ResultType ResultType { get; init; }
    public string? Message { get; init; }
    public Dictionary<string, string[]>? ValidationErrors { get; init; }
    public UserInfoDTO? Data { get; init; }

    private CreateUserResponse(ResultType resultType, string? message, Dictionary<string, string[]>? validationErrors, UserInfoDTO? data)
    {
        ResultType = resultType;
        Message = message;
        ValidationErrors = validationErrors;
        Data = data;
    }

    public static CreateUserResponse Success(UserInfoDTO data) =>
        new(ResultType.Ok, "User created successfully", null, data);

    public static CreateUserResponse DataExists(string message) =>
        new(ResultType.Conflict, message, null, null);

    public static CreateUserResponse Error(string message) =>
        new(ResultType.Error, message, null, null);

    public static CreateUserResponse ValidationError(Dictionary<string, string[]> errors) =>
        new(ResultType.Problems, "Validation errors", errors, null);
}

public class CreateUserService : ICreateUserService
{
    private readonly IAccountsQueriesRepository _accountsQueriesRepository;
    private readonly IUsersCommandsRepository _usersCommandsRepository;
    private readonly IUsersQueriesRepository _usersQueriesRepository;
    private readonly IAccountsUsersCommandsRepository _accountsUsersCommandsRepository;
    private readonly IAccountsUsersQueriesRepository _accountsUsersQueriesRepository;

    public CreateUserService(
        IAccountsQueriesRepository accountsQueriesRepository,
        IUsersCommandsRepository usersCommandsRepository,
        IUsersQueriesRepository usersQueriesRepository,
        IAccountsUsersCommandsRepository accountsUsersCommandsRepository,
        IAccountsUsersQueriesRepository accountsUsersQueriesRepository)
    {
        _accountsQueriesRepository = accountsQueriesRepository;
        _usersCommandsRepository = usersCommandsRepository;
        _usersQueriesRepository = usersQueriesRepository;
        _accountsUsersCommandsRepository = accountsUsersCommandsRepository;
        _accountsUsersQueriesRepository = accountsUsersQueriesRepository;
    }

    public async Task<IGenericResponse<UserInfoDTO>> ExecuteAsync(CreateUserRequest request)
    {
        try
        {
            request.Validate();
        }
        catch (Exception ex)
        {
            return CreateUserResponse.ValidationError(new Dictionary<string, string[]>
            {
                { "Request", new[] { ex.Message } }
            });
        }

        // Verificar que la cuenta existe
        var accountId = Guid.Parse(request.AccountId);
        var accountExists = await _accountsQueriesRepository.GetAll().AnyAsync(a => a.Id == accountId);
        if (!accountExists)
        {
            return CreateUserResponse.Error($"Account with ID '{request.AccountId}' does not exist.");
        }

        // Verificar que el email no existe
        var existUserEmail = await _usersQueriesRepository.GetAll().AnyAsync(u => u.Email == request.Email);
        if (existUserEmail)
        {
            return CreateUserResponse.DataExists($"A user with the email '{request.Email}' already exists.");
        }

        // Verificar que el teléfono no existe
        var existUserPhone = await _usersQueriesRepository.GetAll().AnyAsync(u => u.Phone == request.Phone);
        if (existUserPhone)
        {
            return CreateUserResponse.DataExists($"A user with the phone number '{request.Phone}' already exists.");
        }

        try
        {
            var (user, accountsUsers) = request.ToEntities();

            await _usersCommandsRepository.AddAsync(user);
            await _accountsUsersCommandsRepository.AddAsync(accountsUsers);

            var userDTO = UserMappers.ToDTO(user, accountsUsers.Role);
            return CreateUserResponse.Success(userDTO);
        }
        catch (Exception ex)
        {
            return CreateUserResponse.Error($"An error occurred while creating the user: {ex.Message}");
        }
    }
}
