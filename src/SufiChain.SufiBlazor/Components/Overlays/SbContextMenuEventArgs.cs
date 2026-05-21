namespace SufiChain.SufiBlazor.Components.Overlays;

/// <summary>
/// Event arguments for context menu.
/// </summary>
public class SbContextMenuEventArgs : EventArgs
{
    /// <summary>
    /// X coordinate of the click.
    /// </summary>
    public double X { get; set; }
    
    /// <summary>
    /// Y coordinate of the click.
    /// </summary>
    public double Y { get; set; }
    
    /// <summary>
    /// Set to true to cancel opening the menu.
    /// </summary>
    public bool Cancel { get; set; }
}
