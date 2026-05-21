namespace SufiChain.SufiBlazor.Contracts.Data;

/// <summary>
/// Represents a request for data from a data provider.
/// Used by DataGrid for server-side paging, sorting, and filtering.
/// </summary>
public class SbDataRequest
{
    /// <summary>
    /// The zero-based page index.
    /// </summary>
    public int PageIndex { get; set; }
    
    /// <summary>
    /// The number of items per page.
    /// </summary>
    public int PageSize { get; set; } = 10;
    
    /// <summary>
    /// The sort specifications to apply.
    /// </summary>
    public IReadOnlyList<SbSort> Sorts { get; set; } = Array.Empty<SbSort>();
    
    /// <summary>
    /// The filter specifications to apply.
    /// </summary>
    public IReadOnlyList<SbFilter> Filters { get; set; } = Array.Empty<SbFilter>();
    
    /// <summary>
    /// Global search term (searches across all searchable columns).
    /// </summary>
    public string? Search { get; set; }
    
    /// <summary>
    /// Whether to include the total count in the response.
    /// </summary>
    public bool IncludeTotalCount { get; set; } = true;
    
    /// <summary>
    /// Creates a new data request with default values.
    /// </summary>
    public SbDataRequest() { }
    
    /// <summary>
    /// Creates a new data request with the specified page parameters.
    /// </summary>
    public SbDataRequest(int pageIndex, int pageSize)
    {
        PageIndex = pageIndex;
        PageSize = pageSize;
    }
}
