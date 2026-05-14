namespace TradingProject.Persistence.Api.Stubs.Models;

/// <summary>
/// Represents a paginated list of items.
/// </summary>
/// <typeparam name="T">The type of items in the list.</typeparam>
public class PagingList<T>
{
    public int PageNumber { get; }
    public int PageSize { get; }
    public int TotalCount { get; }
    public T[] Payload { get; }
    public IDictionary<string, object?>? Metadata { get; }

    public PagingList(
        T[] payload,
        int pageNumber,
        int pageSize,
        int totalCount,
        IDictionary<string, object?>? metadata = null)
    {
        Payload = payload;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
        Metadata = metadata;
    }
}
