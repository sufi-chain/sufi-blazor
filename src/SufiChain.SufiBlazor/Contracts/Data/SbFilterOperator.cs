namespace SufiChain.SufiBlazor.Contracts.Data;

/// <summary>
/// Specifies the comparison operator for filter operations.
/// </summary>
public enum SbFilterOperator
{
    /// <summary>
    /// Value equals the filter value.
    /// </summary>
    Equals,
    
    /// <summary>
    /// Value does not equal the filter value.
    /// </summary>
    NotEquals,
    
    /// <summary>
    /// Value contains the filter value (string comparison).
    /// </summary>
    Contains,
    
    /// <summary>
    /// Value starts with the filter value (string comparison).
    /// </summary>
    StartsWith,
    
    /// <summary>
    /// Value ends with the filter value (string comparison).
    /// </summary>
    EndsWith,
    
    /// <summary>
    /// Value is greater than the filter value.
    /// </summary>
    GreaterThan,
    
    /// <summary>
    /// Value is greater than or equal to the filter value.
    /// </summary>
    GreaterThanOrEqual,
    
    /// <summary>
    /// Value is less than the filter value.
    /// </summary>
    LessThan,
    
    /// <summary>
    /// Value is less than or equal to the filter value.
    /// </summary>
    LessThanOrEqual,
    
    /// <summary>
    /// Value is between Value and Value2 (inclusive).
    /// </summary>
    Between,
    
    /// <summary>
    /// Value is in a list of values.
    /// </summary>
    In,
    
    /// <summary>
    /// Value is null.
    /// </summary>
    IsNull,
    
    /// <summary>
    /// Value is not null.
    /// </summary>
    IsNotNull
}
