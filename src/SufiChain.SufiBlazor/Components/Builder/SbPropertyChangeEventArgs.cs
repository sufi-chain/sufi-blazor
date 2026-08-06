namespace SufiChain.SufiBlazor.Components.Builder;

/// <summary>
/// Event arguments for property change events.
/// </summary>
public class SbPropertyChangeEventArgs : EventArgs
{
    /// <summary>
    /// The property definition that changed.
    /// </summary>
    public SbPropertyDefinition Property { get; set; } = null!;
    
    /// <summary>
    /// Property name that changed.
    /// </summary>
    public string PropertyName => Property?.Name ?? string.Empty;
    
    /// <summary>
    /// The old value.
    /// </summary>
    public object? OldValue { get; set; }
    
    /// <summary>
    /// The new value.
    /// </summary>
    public object? NewValue { get; set; }
}
