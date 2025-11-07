namespace Nubulus.Backend.Api.Features.Common;

public interface IPaginatedResponse<T> where T : class
{
    int TotalCount { get; }
    int PageNumber { get; }
    int PageSize { get; }
    List<T> Items { get; }
}
