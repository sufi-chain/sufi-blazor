namespace SufiChain.SufiBlazor.Components.Data;

/// <summary>
/// Defines the row density (spacing) options for the DataGrid.
/// </summary>
public enum SbDataGridDensity
{
    /// <summary>
    /// Compact density with minimal row height (32px).
    /// Best for data-dense displays.
    /// </summary>
    Compact,
    
    /// <summary>
    /// Default density with standard row height (48px).
    /// Balanced for most use cases.
    /// </summary>
    Default,
    
    /// <summary>
    /// Comfortable density with larger row height (56px).
    /// Best for touch interfaces or accessibility.
    /// </summary>
    Comfortable
}
