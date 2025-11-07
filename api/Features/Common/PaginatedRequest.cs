namespace Nubulus.Backend.Api.Features.Common;

public record PaginatedRequest
{
    public int PageNumber { get; init; }
    public int PageSize { get; init; }

    // Constructor per defecto para deserialización
    public PaginatedRequest()
    {
        PageNumber = 1;
        PageSize = 10;
    }

    // Constructor privado con validación
    private PaginatedRequest(int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        Validate();
    }

    public PaginatedRequest Validate()
    {
        if (PageNumber < 1)
            throw new ArgumentException("Page number must be greater than 0.");

        if (PageSize < 1 || PageSize > 100)
            throw new ArgumentException("Page size must be between 1 and 100.");

        return this;
    }

    public static PaginatedRequest Create(int pageNumber, int pageSize)
    {
        return new PaginatedRequest(pageNumber, pageSize);
    }
}
