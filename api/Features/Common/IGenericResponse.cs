namespace Nubulus.Backend.Api.Features.Common;

public interface IGenericResponse<T>
{
    ResultType ResultType { get; }
    string? Message { get; }
    Dictionary<string, string[]>? ValidationErrors { get; }
    T? Data { get; }
}
