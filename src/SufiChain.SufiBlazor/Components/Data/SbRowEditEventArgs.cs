namespace SufiChain.SufiBlazor.Components.Data;

/// <summary>
/// Event arguments for row editing events in SbDataGrid.
/// </summary>
/// <typeparam name="TItem">The type of the data item.</typeparam>
public class SbRowEditEventArgs<TItem>
{
    /// <summary>
    /// Gets the item being edited.
    /// </summary>
    public TItem Item { get; }
    
    /// <summary>
    /// Gets the row key.
    /// </summary>
    public string RowKey { get; }
    
    /// <summary>
    /// Gets the row index.
    /// </summary>
    public int RowIndex { get; }
    
    /// <summary>
    /// Gets or sets whether the operation should be cancelled.
    /// </summary>
    public bool Cancel { get; set; }
    
    /// <summary>
    /// Creates new row edit event arguments.
    /// </summary>
    public SbRowEditEventArgs(TItem item, string rowKey, int rowIndex)
    {
        Item = item;
        RowKey = rowKey;
        RowIndex = rowIndex;
    }
}

/// <summary>
/// Event arguments for row edit completion events.
/// </summary>
/// <typeparam name="TItem">The type of the data item.</typeparam>
public class SbRowEditCompletedEventArgs<TItem> : SbRowEditEventArgs<TItem>
{
    /// <summary>
    /// Gets the original item before editing.
    /// </summary>
    public TItem OriginalItem { get; }
    
    /// <summary>
    /// Gets whether the row was newly inserted.
    /// </summary>
    public bool IsNewRow { get; }
    
    /// <summary>
    /// Creates new row edit completed event arguments.
    /// </summary>
    public SbRowEditCompletedEventArgs(TItem item, TItem originalItem, string rowKey, int rowIndex, bool isNewRow)
        : base(item, rowKey, rowIndex)
    {
        OriginalItem = originalItem;
        IsNewRow = isNewRow;
    }
}

/// <summary>
/// Event arguments for cell editing events.
/// </summary>
/// <typeparam name="TItem">The type of the data item.</typeparam>
public class SbCellEditEventArgs<TItem> : SbRowEditEventArgs<TItem>
{
    /// <summary>
    /// Gets the field being edited.
    /// </summary>
    public string Field { get; }
    
    /// <summary>
    /// Gets the old value before editing.
    /// </summary>
    public object? OldValue { get; }
    
    /// <summary>
    /// Gets or sets the new value after editing.
    /// </summary>
    public object? NewValue { get; set; }
    
    /// <summary>
    /// Creates new cell edit event arguments.
    /// </summary>
    public SbCellEditEventArgs(TItem item, string rowKey, int rowIndex, string field, object? oldValue, object? newValue)
        : base(item, rowKey, rowIndex)
    {
        Field = field;
        OldValue = oldValue;
        NewValue = newValue;
    }
}
