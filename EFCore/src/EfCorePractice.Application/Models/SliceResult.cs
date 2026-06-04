namespace EfCorePractice.Application.Models;

public record SliceResult<T>(
    IReadOnlyList<T> Items,
    int PageNumber,
    int PageSize,
    bool HasNext)
{
    public static SliceResult<T> From(IReadOnlyList<T> items, int pageNumber, int pageSize, bool hasNext) =>
        new(items, pageNumber, pageSize, hasNext);
}
