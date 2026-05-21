namespace SufiChain.SufiBlazor.Contracts.Data;

/// <summary>
/// Represents a filter specification for a data query.
/// </summary>
public class SbFilter
{
    /// <summary>
    /// The field/property name to filter on.
    /// </summary>
    public string Field { get; set; } = string.Empty;
    
    /// <summary>
    /// The filter operator to apply.
    /// </summary>
    public SbFilterOperator Operator { get; set; } = SbFilterOperator.Equals;
    
    /// <summary>
    /// The primary filter value.
    /// </summary>
    public object? Value { get; set; }
    
    /// <summary>
    /// The secondary filter value (used for Between operator).
    /// </summary>
    public object? Value2 { get; set; }
    
    /// <summary>
    /// Creates a new filter specification.
    /// </summary>
    public SbFilter() { }
    
    /// <summary>
    /// Creates a new filter specification with the specified parameters.
    /// </summary>
    public SbFilter(string field, SbFilterOperator @operator, object? value, object? value2 = null)
    {
        Field = field;
        Operator = @operator;
        Value = value;
        Value2 = value2;
    }
}
