namespace Nubulus.Backend.Api.Features.Common;

public record PaginatedResponse<T> : IPaginatedResponse<T> where T : class
{
    public int TotalCount { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public List<T> Items { get; init; }

    public PaginatedResponse(int totalCount, int pageNumber, int pageSize, List<T> items)
    {
        TotalCount = totalCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
        Items = items;
    }
}
