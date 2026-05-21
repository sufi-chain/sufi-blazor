namespace SufiChain.SufiBlazor.Components.Builder;

/// <summary>
/// Event arguments for drop events.
/// </summary>
public class SbDropEventArgs : EventArgs
{
    /// <summary>
    /// X coordinate of the drop.
    /// </summary>
    public double ClientX { get; set; }
    
    /// <summary>
    /// Y coordinate of the drop.
    /// </summary>
    public double ClientY { get; set; }
    
    /// <summary>
    /// The data being dropped.
    /// </summary>
    public object? Data { get; set; }
    
    /// <summary>
    /// The data type being dropped.
    /// </summary>
    public string? DataType { get; set; }
}
