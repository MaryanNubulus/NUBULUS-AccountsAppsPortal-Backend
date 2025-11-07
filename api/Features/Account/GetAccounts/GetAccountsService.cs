using Microsoft.EntityFrameworkCore;
using Nubulus.Backend.Api.Features.Account.Common;
using Nubulus.Backend.Api.Features.Common;
using Nubulus.Domain.Abstractions;

namespace Nubulus.Backend.Api.Features.Account.GetAccounts;

public class GetAccountsService
{
    internal class GetAccountsResponse : IGenericResponse<PaginatedResponse<AccountDto>>
    {
        public ResultType ResultType { get; set; } = ResultType.None;
        public string? Message { get; set; } = null;
        public Dictionary<string, string[]>? ValidationErrors { get; set; } = null;
        public PaginatedResponse<AccountDto>? Data { get; set; } = null;

        private GetAccountsResponse(ResultType resultType, string? message,
                                    Dictionary<string, string[]>? validationErrors,
                                    PaginatedResponse<AccountDto>? data)
        {
            ResultType = resultType;
            Message = message;
            ValidationErrors = validationErrors;
            Data = data;
        }

        public static GetAccountsResponse Success(PaginatedResponse<AccountDto> data)
        {
            return new GetAccountsResponse(ResultType.Ok, null, null, data);
        }

        public static GetAccountsResponse NotFound(string message)
        {
            return new GetAccountsResponse(ResultType.NotFound, message, null, null);
        }

        public static GetAccountsResponse Error(string message)
        {
            return new GetAccountsResponse(ResultType.Error, message, null, null);
        }

        public static GetAccountsResponse ValidationError(Dictionary<string, string[]> errors)
        {
            return new GetAccountsResponse(ResultType.Problems, "Validation errors occurred.", errors, null);
        }
    }

    private readonly IAccountsRepository _accountsRepository;

    public GetAccountsService(IAccountsRepository accountsRepository)
    {
        _accountsRepository = accountsRepository;
    }

    public async Task<IGenericResponse<PaginatedResponse<AccountDto>>> ExecuteAsync(PaginatedRequest request, CancellationToken cancellationToken)
    {
        // 1. Validar request
        try { request.Validate(); }
        catch (Exception ex)
        {
            return GetAccountsResponse.ValidationError(new Dictionary<string, string[]>
            {
                { "Request", new[] { ex.Message } }
            });
        }

        try
        {
            // Obtener IQueryable de modelos de persistencia (permite paginación en BD)
            var accountsQuery = ((Nubulus.Backend.Infraestructure.Pgsql.Repositories.AccountRepository)_accountsRepository)
                .GetAllAccountsQueryable();

            // Obtener el total de registros
            var totalCount = await accountsQuery.CountAsync(cancellationToken);

            if (totalCount == 0)
            {
                return GetAccountsResponse.NotFound("No accounts found.");
            }

            // Aplicar paginación en la base de datos
            var accountModels = await accountsQuery
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            // Mapear modelos de persistencia a entidades de dominio
            var accountEntities = accountModels.Select(a => new Domain.Entities.Account.AccountEntity
            {
                Id = a.Id,
                AccountKey = new Domain.ValueObjects.AccountKey(a.Key),
                Name = a.Name,
                Email = new Domain.ValueObjects.EmailAddress(a.Email),
                Phone = a.Phone,
                Status = a.Status == "A"
                    ? Domain.ValueObjects.AccountStatus.Active
                    : Domain.ValueObjects.AccountStatus.Inactive
            }).ToList();

            // Mapear entidades de dominio a DTOs
            var accountDtos = accountEntities.ToDto();

            // Crear response paginado genérico
            var paginatedResponse = new PaginatedResponse<AccountDto>(
                totalCount: totalCount,
                pageNumber: request.PageNumber,
                pageSize: request.PageSize,
                items: accountDtos
            );

            return GetAccountsResponse.Success(paginatedResponse);
        }
        catch (Exception ex)
        {
            return GetAccountsResponse.Error($"An error occurred while retrieving accounts: {ex.Message}");
        }
    }
}
