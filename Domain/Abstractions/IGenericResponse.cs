namespace NUBULUS.AccountsAppsPortalBackEnd.Domain.Abstractions;

public interface IGenericResponse<T> where T : class
{
    bool Success { get; set; }
    string? Message { get; set; }
    T? Data { get; set; }
}