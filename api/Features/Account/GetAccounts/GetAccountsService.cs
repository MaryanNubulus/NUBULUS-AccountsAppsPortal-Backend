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
        public PaginatedResponse<AccountDto>? Data { get; set; } = null;

        public Dictionary<string, string[]>? ValidationErrors { get; set; } = null;

        private GetAccountsResponse(ResultType resultType, string? message,
                                    PaginatedResponse<AccountDto>? data)
        {
            ResultType = resultType;
            Message = message;
            Data = data;
            ValidationErrors = null;
        }

        public static GetAccountsResponse Success(PaginatedResponse<AccountDto> data)
        {
            return new GetAccountsResponse(ResultType.Ok, null, data);
        }

        public static GetAccountsResponse NotFound(string message)
        {
            return new GetAccountsResponse(ResultType.NotFound, message, null);
        }

        public static GetAccountsResponse Error(string message)
        {
            return new GetAccountsResponse(ResultType.Error, message, null);
        }
    }

    private readonly IAccountsRepository _accountsRepository;

    public GetAccountsService(IAccountsRepository accountsRepository)
    {
        _accountsRepository = accountsRepository;
    }

    public async Task<IGenericResponse<PaginatedResponse<AccountDto>>> ExecuteAsync(GetAccountsRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var accountsQuery = await _accountsRepository.GetAccountsAsync(request.SearchTerm, cancellationToken);

            var totalCount = await accountsQuery.CountAsync(cancellationToken);

            if (totalCount == 0)
            {
                return GetAccountsResponse.NotFound("No accounts found.");
            }

            var accountEntities = await accountsQuery
                .Skip(((request.PageNumber ?? 1) - 1) * (request.PageSize ?? 10))
                .Take(request.PageSize ?? 10)
                .ToListAsync(cancellationToken);

            var accountDtos = accountEntities.ToDto();

            var paginatedResponse = new PaginatedResponse<AccountDto>(
                totalCount: totalCount,
                pageNumber: request.PageNumber ?? 1,
                pageSize: request.PageSize ?? 10,
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
