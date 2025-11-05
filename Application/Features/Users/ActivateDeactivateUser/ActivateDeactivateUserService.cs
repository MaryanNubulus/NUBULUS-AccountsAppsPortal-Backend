using MongoDB.Driver.Linq;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.DTOs;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Enums;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Abstractions;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Users.Common;
using NUBULUS.AccountsAppsPortalBackEnd.Domain.Entities;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Users.ActivateDeactivateUser;

internal sealed class ActivateDeactivateUserResponse : IGenericResponse<UserInfoDTO>
{
    public ResultType ResultType { get; init; }
    public string? Message { get; init; }
    public Dictionary<string, string[]>? ValidationErrors { get; init; }
    public UserInfoDTO? Data { get; init; }

    private ActivateDeactivateUserResponse(ResultType resultType, string? message, Dictionary<string, string[]>? validationErrors, UserInfoDTO? data)
    {
        ResultType = resultType;
        Message = message;
        ValidationErrors = validationErrors;
        Data = data;
    }

    public static ActivateDeactivateUserResponse Success(UserInfoDTO data) =>
        new(ResultType.Ok, "User status changed successfully", null, data);

    public static ActivateDeactivateUserResponse Error(string message) =>
        new(ResultType.Error, message, null, null);

    public static ActivateDeactivateUserResponse ValidationError(Dictionary<string, string[]> errors) =>
        new(ResultType.Problems, "Validation errors", errors, null);
}

public class ActivateDeactivateUserService : IActivateDeactivateUserService
{
    private readonly IAccountsQueriesRepository _accountsQueriesRepository;
    private readonly IUsersCommandsRepository _usersCommandsRepository;
    private readonly IUsersQueriesRepository _usersQueriesRepository;
    private readonly IAccountsUsersQueriesRepository _accountsUsersQueriesRepository;

    public ActivateDeactivateUserService(
        IAccountsQueriesRepository accountsQueriesRepository,
        IUsersCommandsRepository usersCommandsRepository,
        IUsersQueriesRepository usersQueriesRepository,
        IAccountsUsersQueriesRepository accountsUsersQueriesRepository)
    {
        _accountsQueriesRepository = accountsQueriesRepository;
        _usersCommandsRepository = usersCommandsRepository;
        _usersQueriesRepository = usersQueriesRepository;
        _accountsUsersQueriesRepository = accountsUsersQueriesRepository;
    }

    public async Task<IGenericResponse<UserInfoDTO>> ExecuteAsync(string accountId, string userId, bool activate)
    {
        // Validar IDs
        if (!Guid.TryParse(accountId, out var accountGuid))
        {
            return ActivateDeactivateUserResponse.ValidationError(new Dictionary<string, string[]>
            {
                { "Request", new[] { "Invalid account ID format." } }
            });
        }

        if (!Guid.TryParse(userId, out var userGuid))
        {
            return ActivateDeactivateUserResponse.ValidationError(new Dictionary<string, string[]>
            {
                { "Request", new[] { "Invalid user ID format." } }
            });
        }

        // Verificar que la cuenta existe
        var accountExists = await _accountsQueriesRepository.GetAll().AnyAsync(a => a.Id == accountGuid);
        if (!accountExists)
        {
            return ActivateDeactivateUserResponse.Error($"Account with ID '{accountId}' does not exist.");
        }

        // Verificar que el usuario existe
        var user = await _usersQueriesRepository.GetAll().FirstOrDefaultAsync(u => u.Id == userGuid);
        if (user == null)
        {
            return ActivateDeactivateUserResponse.Error($"User with ID '{userId}' does not exist.");
        }

        // Verificar que el usuario pertenece a la cuenta
        var accountUser = await _accountsUsersQueriesRepository.GetAll()
            .FirstOrDefaultAsync(au => au.AccountId == accountGuid && au.UserId == userGuid);
        if (accountUser == null)
        {
            return ActivateDeactivateUserResponse.Error($"User with ID '{userId}' does not belong to account '{accountId}'.");
        }

        // Verificar que no se intenta desactivar un Owner
        if (accountUser.Role == AccountsUsersRole.Owner)
        {
            return ActivateDeactivateUserResponse.Error("Cannot activate/deactivate the Owner user.");
        }

        try
        {
            user.IsActive = activate;
            await _usersCommandsRepository.UpdateAsync(user.Id, user);

            var userDTO = UserMappers.ToDTO(user, accountUser.Role);
            return ActivateDeactivateUserResponse.Success(userDTO);
        }
        catch (Exception ex)
        {
            return ActivateDeactivateUserResponse.Error($"An error occurred while changing user status: {ex.Message}");
        }
    }
}
