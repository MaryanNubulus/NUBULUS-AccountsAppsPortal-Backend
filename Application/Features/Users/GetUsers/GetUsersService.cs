using MongoDB.Driver.Linq;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.DTOs;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Enums;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Abstractions;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Users.Common;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Users.GetUsers;

internal sealed class GetUsersResponse : IGenericResponse<IEnumerable<UserInfoDTO>>
{
    public ResultType ResultType { get; init; }
    public string? Message { get; init; }
    public Dictionary<string, string[]>? ValidationErrors { get; init; }
    public IEnumerable<UserInfoDTO>? Data { get; init; }

    private GetUsersResponse(ResultType resultType, string? message, Dictionary<string, string[]>? validationErrors, IEnumerable<UserInfoDTO>? data)
    {
        ResultType = resultType;
        Message = message;
        ValidationErrors = validationErrors;
        Data = data;
    }

    public static GetUsersResponse Success(IEnumerable<UserInfoDTO> data) =>
        new(ResultType.Ok, "Users retrieved successfully", null, data);

    public static GetUsersResponse Error(string message) =>
        new(ResultType.Error, message, null, null);

    public static GetUsersResponse ValidationError(Dictionary<string, string[]> errors) =>
        new(ResultType.Problems, "Validation errors", errors, null);
}

public class GetUsersService : IGetUsersService
{
    private readonly IAccountsQueriesRepository _accountsQueriesRepository;
    private readonly IUsersQueriesRepository _usersQueriesRepository;
    private readonly IAccountsUsersQueriesRepository _accountsUsersQueriesRepository;

    public GetUsersService(
        IAccountsQueriesRepository accountsQueriesRepository,
        IUsersQueriesRepository usersQueriesRepository,
        IAccountsUsersQueriesRepository accountsUsersQueriesRepository)
    {
        _accountsQueriesRepository = accountsQueriesRepository;
        _usersQueriesRepository = usersQueriesRepository;
        _accountsUsersQueriesRepository = accountsUsersQueriesRepository;
    }

    public async Task<IGenericResponse<IEnumerable<UserInfoDTO>>> ExecuteAsync(string accountId)
    {
        try
        {
            if (!Guid.TryParse(accountId, out var accountGuid))
            {
                return GetUsersResponse.ValidationError(new Dictionary<string, string[]>
                {
                    { "Request", new[] { "Invalid account ID format." } }
                });
            }

            // Verificar que la cuenta existe
            var accountExists = await _accountsQueriesRepository.GetAll().AnyAsync(a => a.Id == accountGuid);
            if (!accountExists)
            {
                return GetUsersResponse.Error($"Account with ID '{accountId}' does not exist.");
            }

            // Obtener las relaciones AccountsUsers para la cuenta
            var accountsUsers = await _accountsUsersQueriesRepository
                .GetAll()
                .Where(au => au.AccountId == accountGuid)
                .ToListAsync();

            if (!accountsUsers.Any())
            {
                return GetUsersResponse.Success(Enumerable.Empty<UserInfoDTO>());
            }

            // Obtener los IDs de usuarios
            var userIds = accountsUsers.Select(au => au.UserId).ToList();

            // Obtener los usuarios
            var users = await _usersQueriesRepository
                .GetAll()
                .Where(u => userIds.Contains(u.Id))
                .ToListAsync();

            var userDTOs = UserMappers.ToDTOList(users, accountsUsers);

            return GetUsersResponse.Success(userDTOs);
        }
        catch (Exception ex)
        {
            return GetUsersResponse.Error($"An error occurred while retrieving users: {ex.Message}");
        }
    }
}
