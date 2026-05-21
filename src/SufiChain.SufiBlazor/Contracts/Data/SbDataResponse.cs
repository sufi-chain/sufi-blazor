namespace SufiChain.SufiBlazor.Contracts.Data;

/// <summary>
/// Represents a response from a data provider.
/// Contains the requested items and optional total count for pagination.
/// </summary>
/// <typeparam name="T">The type of items in the response.</typeparam>
public class SbDataResponse<T>
{
    /// <summary>
    /// The items returned by the data provider.
    /// </summary>
    public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();
    
    /// <summary>
    /// The total count of items matching the query (before pagination).
    /// Null if total count was not requested or is unknown.
    /// </summary>
    public long? TotalCount { get; set; }
    
    /// <summary>
    /// Creates a new empty data response.
    /// </summary>
    public SbDataResponse() { }
    
    /// <summary>
    /// Creates a new data response with the specified items.
    /// </summary>
    public SbDataResponse(IReadOnlyList<T> items, long? totalCount = null)
    {
        Items = items;
        TotalCount = totalCount;
    }
    
}
