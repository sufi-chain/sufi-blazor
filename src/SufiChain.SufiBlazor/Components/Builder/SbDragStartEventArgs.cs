namespace SufiChain.SufiBlazor.Components.Builder;

/// <summary>
/// Event arguments for drag start events.
/// </summary>
public class SbDragStartEventArgs : EventArgs
{
    /// <summary>
    /// The item being dragged.
    /// </summary>
    public object? Item { get; set; }
    
    /// <summary>
    /// The data being dragged.
    /// </summary>
    public object? Data { get; set; }
    
    /// <summary>
    /// The item type.
    /// </summary>
    public string? ItemType { get; set; }
    
    /// <summary>
    /// Index of the item being dragged.
    /// </summary>
    public int Index { get; set; }
    
    /// <summary>
    /// Set to true to cancel the drag.
    /// </summary>
    public bool Cancel { get; set; }
}
