namespace TradingProject.Persistence.Application.Common.Models;

/// <summary>
/// Represents a paginated list of items.
/// </summary>
/// <typeparam name="T">The type of items in the list.</typeparam>
/// <param name="payload">The items for the current page.</param>
/// <param name="pageNumber">The current page number (1-based).</param>
/// <param name="pageSize">The maximum number of items per page.</param>
/// <param name="totalCount">The total number of items across all pages.</param>
/// <param name="metadata">Optional metadata associated with the paging result.</param>
public class PagingList<T>(
    T[] payload,
    int pageNumber,
    int pageSize,
    int totalCount,
    IDictionary<string, object?>? metadata = null)
{
    /// <summary>
    /// Gets the current page number (1-based, starts from 1).
    /// </summary>
    public int PageNumber { get; } = pageNumber;

    /// <summary>
    /// Gets the maximum number of items per page.
    /// </summary>
    public int PageSize { get; } = pageSize;

    /// <summary>
    /// Gets the total number of items across all pages.
    /// </summary>
    public int TotalCount { get; } = totalCount;

    /// <summary>
    /// Gets the items for the current page.
    /// </summary>
    public T[] Payload { get; } = payload;

    /// <summary>
    /// Gets optional metadata associated with the paging result.
    /// </summary>
    public IDictionary<string, object?>? Metadata { get; } = metadata;
}   