using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Enums;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Abstractions;

public interface IGenericResponse<T> where T : class
{
    ResultType ResultType { get; }
    string? Message { get; }
    Dictionary<string, string[]>? ValidationErrors { get; }
    T? Data { get; }
}
