namespace SufiChain.SufiBlazor.Components.Data;

/// <summary>
/// Represents an active filter tag.
/// </summary>
public class SbFilterTag
{
    /// <summary>
    /// Unique identifier for the filter.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// Key/identifier for the filter (alias for Id).
    /// </summary>
    public string Key
    {
        get => Id;
        set => Id = value;
    }
    
    /// <summary>
    /// Display label for the filter.
    /// </summary>
    public string Label { get; set; } = string.Empty;
    
    /// <summary>
    /// The filter value.
    /// </summary>
    public object? Value { get; set; }
    
    /// <summary>
    /// The field this filter applies to.
    /// </summary>
    public string? Field { get; set; }
    
    /// <summary>
    /// Creates a new filter tag.
    /// </summary>
    public SbFilterTag() { }
    
    /// <summary>
    /// Creates a new filter tag with label and value.
    /// </summary>
    public SbFilterTag(string label, object? value = null, string? field = null)
    {
        Label = label;
        Value = value;
        Field = field;
    }
}
