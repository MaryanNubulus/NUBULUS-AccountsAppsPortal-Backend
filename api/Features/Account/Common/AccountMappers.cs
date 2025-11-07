using Nubulus.Domain.Entities.Account;

namespace Nubulus.Backend.Api.Features.Account.Common;

public static class AccountMappers
{
    public static AccountDto ToDto(this AccountEntity entity)
    {
        return new AccountDto
        {
            Key = entity.AccountKey.Value,
            Name = entity.Name,
            FullName = entity.FullName,
            Email = entity.Email.Value,
            Phone = entity.Phone,
            NumberId = entity.NumberId,
            Status = entity.Status.Value
        };
    }

    public static List<AccountDto> ToDto(this IEnumerable<AccountEntity> entities)
    {
        return entities.Select(e => e.ToDto()).ToList();
    }
}
