using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.DTOs;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Requests;
using NUBULUS.AccountsAppsPortalBackEnd.Domain.Entities;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Users.Common;

public static class UserMappers
{
    public static UserInfoDTO ToDTO(User user, AccountsUsersRole role)
    {
        return new UserInfoDTO
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Phone = user.Phone,
            IsActive = user.IsActive,
            Role = role.ToString()
        };
    }

    public static (User, AccountsUsers) ToEntities(this CreateUserRequest request)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Email = request.Email,
            Phone = request.Phone,
            IsActive = true
        };

        var role = Enum.Parse<AccountsUsersRole>(request.Role, ignoreCase: true);

        var accountsUsers = new AccountsUsers
        {
            Id = Guid.NewGuid(),
            AccountId = Guid.Parse(request.AccountId),
            UserId = user.Id,
            Role = role
        };

        return (user, accountsUsers);
    }

    public static IEnumerable<UserInfoDTO> ToDTOList(
        IEnumerable<User> users,
        IEnumerable<AccountsUsers> accountsUsers)
    {
        var accountUserDict = accountsUsers.ToDictionary(au => au.UserId, au => au.Role);

        foreach (var user in users)
        {
            if (accountUserDict.TryGetValue(user.Id, out var role))
            {
                yield return ToDTO(user, role);
            }
        }
    }
}
