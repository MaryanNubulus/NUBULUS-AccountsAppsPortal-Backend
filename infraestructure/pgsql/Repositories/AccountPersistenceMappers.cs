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
            Status = model.Status == "A"
                ? AccountStatus.Active
                : AccountStatus.Inactive
        };
    }

    public static List<AccountEntity> ToEntity(this IEnumerable<Account> models)
    {
        return models.Select(m => m.ToEntity()).ToList();
    }
}
