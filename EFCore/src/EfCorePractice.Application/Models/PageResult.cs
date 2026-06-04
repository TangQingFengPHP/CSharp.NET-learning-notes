namespace EfCorePractice.Application.Models;

public record PageResult<T>(
    IReadOnlyList<T> Items,
    long TotalCount,
    int PageNumber,
    int PageSize)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    public static PageResult<T> From(IReadOnlyList<T> items, long totalCount, int pageNumber, int pageSize) =>
        new(items, totalCount, pageNumber, pageSize);
}
