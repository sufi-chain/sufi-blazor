namespace SufiChain.SufiBlazor.Contracts.Data;

/// <summary>
/// Represents a sort specification for a data query.
/// </summary>
public class SbSort
{
    /// <summary>
    /// The field/property name to sort by.
    /// </summary>
    public string Field { get; set; } = string.Empty;
    
    /// <summary>
    /// The sort direction.
    /// </summary>
    public SbSortDirection Direction { get; set; } = SbSortDirection.Ascending;
    
    /// <summary>
    /// Creates a new sort specification.
    /// </summary>
    public SbSort() { }
    
    /// <summary>
    /// Creates a new sort specification with the specified field and direction.
    /// </summary>
    public SbSort(string field, SbSortDirection direction = SbSortDirection.Ascending)
    {
        Field = field;
        Direction = direction;
    }
}
