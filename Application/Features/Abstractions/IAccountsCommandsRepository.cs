using MongoDB.Driver;
using NUBULUS.AccountsAppsPortalBackEnd.Domain.Abstractions;
using NUBULUS.AccountsAppsPortalBackEnd.Domain.Entities;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Abstractions;

public interface IAccountsCommandsRepository : ICommand, IGenericCommandsRepository<Guid, Account>
{
}