using MongoDB.Driver.Linq;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.DTOs;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Enums;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Requests;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Abstractions;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Users.Common;
using NUBULUS.AccountsAppsPortalBackEnd.Domain.Entities;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Users.UpdateUser;

internal sealed class UpdateUserResponse : IGenericResponse<UserInfoDTO>
{
    public ResultType ResultType { get; init; }
    public string? Message { get; init; }
    public Dictionary<string, string[]>? ValidationErrors { get; init; }
    public UserInfoDTO? Data { get; init; }

    private UpdateUserResponse(ResultType resultType, string? message, Dictionary<string, string[]>? validationErrors, UserInfoDTO? data)
    {
        ResultType = resultType;
        Message = message;
        ValidationErrors = validationErrors;
        Data = data;
    }

    public static UpdateUserResponse Success(UserInfoDTO data) =>
        new(ResultType.Ok, "User updated successfully", null, data);

    public static UpdateUserResponse DataExists(string message) =>
        new(ResultType.Conflict, message, null, null);

    public static UpdateUserResponse Error(string message) =>
        new(ResultType.Error, message, null, null);

    public static UpdateUserResponse ValidationError(Dictionary<string, string[]> errors) =>
        new(ResultType.Problems, "Validation errors", errors, null);
}

public class UpdateUserService : IUpdateUserService
{
    private readonly IAccountsQueriesRepository _accountsQueriesRepository;
    private readonly IUsersCommandsRepository _usersCommandsRepository;
    private readonly IUsersQueriesRepository _usersQueriesRepository;
    private readonly IAccountsUsersCommandsRepository _accountsUsersCommandsRepository;
    private readonly IAccountsUsersQueriesRepository _accountsUsersQueriesRepository;

    public UpdateUserService(
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

    public async Task<IGenericResponse<UserInfoDTO>> ExecuteAsync(string accountId, string userId, UpdateUserRequest request)
    {
        try
        {
            request.Validate();
        }
        catch (Exception ex)
        {
            return UpdateUserResponse.ValidationError(new Dictionary<string, string[]>
            {
                { "Request", new[] { ex.Message } }
            });
        }

        // Validar IDs
        if (!Guid.TryParse(accountId, out var accountGuid))
        {
            return UpdateUserResponse.ValidationError(new Dictionary<string, string[]>
            {
                { "Request", new[] { "Invalid account ID format." } }
            });
        }

        if (!Guid.TryParse(userId, out var userGuid))
        {
            return UpdateUserResponse.ValidationError(new Dictionary<string, string[]>
            {
                { "Request", new[] { "Invalid user ID format." } }
            });
        }

        // Verificar que la cuenta existe
        var accountExists = await _accountsQueriesRepository.GetAll().AnyAsync(a => a.Id == accountGuid);
        if (!accountExists)
        {
            return UpdateUserResponse.Error($"Account with ID '{accountId}' does not exist.");
        }

        // Verificar que el usuario existe
        var user = await _usersQueriesRepository.GetAll().FirstOrDefaultAsync(u => u.Id == userGuid);
        if (user == null)
        {
            return UpdateUserResponse.Error($"User with ID '{userId}' does not exist.");
        }

        // Verificar que el usuario pertenece a la cuenta
        var accountUser = await _accountsUsersQueriesRepository.GetAll()
            .FirstOrDefaultAsync(au => au.AccountId == accountGuid && au.UserId == userGuid);
        if (accountUser == null)
        {
            return UpdateUserResponse.Error($"User with ID '{userId}' does not belong to account '{accountId}'.");
        }

        // Verificar que no se intenta cambiar el rol de un Owner
        if (accountUser.Role == AccountsUsersRole.Owner && request.Role != "Owner")
        {
            return UpdateUserResponse.Error("Cannot modify the Owner user.");
        }

        // Verificar que el email no existe en otro usuario
        var existUserEmail = await _usersQueriesRepository.GetAll()
            .AnyAsync(u => u.Email == request.Email && u.Id != userGuid);
        if (existUserEmail)
        {
            return UpdateUserResponse.DataExists($"A user with the email '{request.Email}' already exists.");
        }

        // Verificar que el teléfono no existe en otro usuario
        var existUserPhone = await _usersQueriesRepository.GetAll()
            .AnyAsync(u => u.Phone == request.Phone && u.Id != userGuid);
        if (existUserPhone)
        {
            return UpdateUserResponse.DataExists($"A user with the phone number '{request.Phone}' already exists.");
        }

        try
        {
            // Actualizar usuario
            user.Name = request.Name;
            user.Email = request.Email;
            user.Phone = request.Phone;

            await _usersCommandsRepository.UpdateAsync(user.Id, user);

            // Actualizar rol si cambió
            var newRole = Enum.Parse<AccountsUsersRole>(request.Role, ignoreCase: true);
            if (accountUser.Role != newRole)
            {
                accountUser.Role = newRole;
                await _accountsUsersCommandsRepository.UpdateAsync(accountUser.Id, accountUser);
            }

            var userDTO = UserMappers.ToDTO(user, newRole);
            return UpdateUserResponse.Success(userDTO);
        }
        catch (Exception ex)
        {
            return UpdateUserResponse.Error($"An error occurred while updating the user: {ex.Message}");
        }
    }
}
