using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.DTOs;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Requests;
using NUBULUS.AccountsAppsPortalBackEnd.Domain.Entities;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Accounts.Common;

public static class AccountMappers
{
    public static AccountInfoDTO ToDTO(Account account, User user)
    {
        return new AccountInfoDTO
        {
            Id = account.Id,
            Name = account.Name,
            IsActive = account.IsActive,
            UserName = user.Name,
            UserEmail = user.Email,
            UserPhone = user.Phone
        };
    }

    public static (Account, User, AccountsUsers) ToEntities(this CreateAccountRequest request)
    {
        var account = new Account
        {
            Id = Guid.NewGuid(),
            Name = request.AccountName,
            IsActive = true
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = request.UserName,
            Email = request.UserEmail,
            Phone = request.UserPhone,
            IsActive = true
        };

        var accountsUsers = new AccountsUsers
        {
            Id = Guid.NewGuid(),
            AccountId = account.Id,
            UserId = user.Id,
            Role = AccountsUsersRole.Owner
        };

        return (account, user, accountsUsers);
    }

    public static IEnumerable<AccountInfoDTO> ToDTOList(
    IEnumerable<Account> accounts,
    IEnumerable<User> users,
    IEnumerable<AccountsUsers> accountsUsers)
    {
        var userDict = users.ToDictionary(u => u.Id);

        var accountUserDict = accountsUsers
            .GroupBy(au => au.AccountId)
            .ToDictionary(
                g => g.Key,
                g => g.Select(au => au.UserId).ToList()
            );

        foreach (var account in accounts)
        {
            if (accountUserDict.TryGetValue(account.Id, out var userIds))
            {
                foreach (var userId in userIds)
                {
                    if (userDict.TryGetValue(userId, out var user))
                    {
                        yield return ToDTO(account, user);
                    }
                }
            }
        }
    }

}