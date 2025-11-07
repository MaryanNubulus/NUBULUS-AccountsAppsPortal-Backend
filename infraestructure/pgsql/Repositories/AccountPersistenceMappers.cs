using Nubulus.Backend.Infraestructure.PostgreSQL.Models;
using Nubulus.Domain.Entities.Account;
using Nubulus.Domain.ValueObjects;

namespace Nubulus.Backend.Infraestructure.Pgsql.Repositories;

public static class AccountPersistenceMappers
{
    public static AccountEntity ToEntity(this Account model)
    {
        return new AccountEntity
        {
            Id = model.Id,
            AccountKey = new AccountKey(model.Key),
            Name = model.Name,
            Email = new EmailAddress(model.Email),
            Phone = model.Phone,
            NumberId = model.NumberId,
            Status = model.Status == "A"
                ? AccountStatus.Active
                : AccountStatus.Inactive
        };
    }

    public static List<AccountEntity> ToEntities(this IEnumerable<Account> models)
    {
        return models.Select(m => m.ToEntity()).ToList();
    }

    public static Account ToModel(this AccountEntity entity)
    {
        return new Account
        {
            Id = entity.Id,
            Key = entity.AccountKey.Value,
            Name = entity.Name,
            Email = entity.Email.Value,
            Phone = entity.Phone,
            NumberId = entity.NumberId,
            Status = entity.Status == AccountStatus.Active ? "A" : "I"
        };
    }

    public static List<Account> ToModels(this IEnumerable<AccountEntity> entities)
    {
        return entities.Select(e => e.ToModel()).ToList();
    }
}
