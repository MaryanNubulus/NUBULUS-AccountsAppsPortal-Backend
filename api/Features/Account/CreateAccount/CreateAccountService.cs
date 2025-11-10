using System.Text;
using System.Text.Json;
using Nubulus.Backend.Api.Features.Common;
using Nubulus.Backend.Infraestructure.Pgsql.Models;
using Nubulus.Domain.Abstractions;
using Nubulus.Domain.Entities.AuditRecord;
using Nubulus.Domain.ValueObjects;

namespace Nubulus.Backend.Api.Features.Account;

public class CreateAccountService
{
    internal class CreateAccountResponse : IGenericResponse<string>
    {
        public ResultType ResultType { get; set; } = ResultType.None;

        public string? Message { get; set; } = null;

        public Dictionary<string, string[]>? ValidationErrors { get; set; } = null;

        public string? Data { get; set; } = null;

        private CreateAccountResponse(ResultType resultType, string? message, Dictionary<string, string[]>? validationErrors, string? data)
        {
            ResultType = resultType;
            Message = message;
            ValidationErrors = validationErrors;
            Data = data;
        }
        public static CreateAccountResponse Success(string data)
        {
            return new CreateAccountResponse(ResultType.Ok, null, null, data);
        }
        public static CreateAccountResponse DataExists(string message)
        {
            return new CreateAccountResponse(ResultType.Conflict, message, null, null);
        }
        public static CreateAccountResponse Error(string message)
        {
            return new CreateAccountResponse(ResultType.Error, message, null, null);
        }
        public static CreateAccountResponse ValidationError(Dictionary<string, string[]> validationErrors)
        {
            return new CreateAccountResponse(ResultType.Problems, "Validation errors occurred.", validationErrors, null);
        }
    }

    private readonly IAccountsRepository _accountsRepository;
    private readonly IAuditRecordRepository _auditRecordRepository;
    public CreateAccountService(IAccountsRepository accountsRepository, IAuditRecordRepository auditRecordRepository)
    {
        _accountsRepository = accountsRepository;
        _auditRecordRepository = auditRecordRepository;
    }

    public async Task<IGenericResponse<string>> ExecuteAsync(CreateAccountRequest request, string userContext, CancellationToken cancellationToken)
    {
        if (request.Validate().Count > 0)
        {
            return CreateAccountResponse.ValidationError(request.Validate());
        }

        var existingAccount = await _accountsRepository.AccountInfoExistsAsync(request.Name, request.Email, request.Phone, request.NumberId, cancellationToken);

        if (existingAccount)
        {
            return CreateAccountResponse.DataExists("An account with the same Name, Email, Phone, or NumberId already exists.");
        }
        var accountKey = Guid.NewGuid().ToString();
        var userKey = Guid.NewGuid().ToString();
        var accountUserKey = Guid.NewGuid().ToString();
        try
        {
            var command = new Domain.Entities.Account.CreateAccount(
                accountKey,
                userKey,
                accountUserKey,
                request.Name,
                request.FullName,
                new EmailAddress(request.Email),
                request.Phone,
                request.Address,
                request.NumberId
            );

            await _accountsRepository.CreateAccountAsync(command, cancellationToken);


            var commandJson = JsonSerializer.Serialize(command);
            var commandBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(commandJson));

            var accountAuditRecord = new CreateAuditRecord($"{nameof(Account)}s".ToLower(), accountKey, RecordType.Create, userContext, commandBase64);
            var userAuditRecord = new CreateAuditRecord($"{nameof(User)}s".ToLower(), userKey, RecordType.Create, userContext, commandBase64);
            var accountUserAuditRecord = new CreateAuditRecord($"{nameof(AccountUser)}s".ToLower(), accountUserKey, RecordType.Create, userContext, commandBase64);

            await _auditRecordRepository.CreateAuditRecordAsync(accountAuditRecord, cancellationToken);
            await _auditRecordRepository.CreateAuditRecordAsync(userAuditRecord, cancellationToken);
            await _auditRecordRepository.CreateAuditRecordAsync(accountUserAuditRecord, cancellationToken);
        }
        catch (Exception ex)
        {
            return CreateAccountResponse.Error($"An error occurred while creating the account: {ex.Message}");
        }

        return CreateAccountResponse.Success(accountKey);
    }
}