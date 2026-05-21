namespace SufiChain.SufiBlazor.Components.Data;

/// <summary>
/// Specifies the edit mode for SbDataGrid.
/// </summary>
public enum SbEditMode
{
    /// <summary>
    /// Editing is disabled.
    /// </summary>
    None,
    
    /// <summary>
    /// Edit individual cells inline when clicked or when Enter is pressed.
    /// </summary>
    Cell,
    
    /// <summary>
    /// Edit the entire row inline when the row enters edit mode.
    /// </summary>
    Row,
    
    /// <summary>
    /// Edit the row in a popup/dialog form.
    /// </summary>
    Popup
}
