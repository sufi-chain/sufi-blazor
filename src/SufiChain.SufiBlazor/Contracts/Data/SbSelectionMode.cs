namespace SufiChain.SufiBlazor.Contracts.Data;

/// <summary>
/// Specifies the selection mode for data components like DataGrid.
/// </summary>
public enum SbSelectionMode
{
    /// <summary>
    /// Selection is disabled.
    /// </summary>
    None,
    
    /// <summary>
    /// Only one item can be selected at a time.
    /// </summary>
    SingleRow,
    
    /// <summary>
    /// Multiple items can be selected.
    /// </summary>
    MultipleRows
}
