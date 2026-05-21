namespace SufiChain.SufiBlazor.Components.Data;

/// <summary>
/// Context provided to cell templates in DataGrid.
/// </summary>
/// <typeparam name="TItem">The type of the data item.</typeparam>
public class SbCellContext<TItem>
{
    /// <summary>
    /// The data item for the current row.
    /// </summary>
    public TItem Item { get; }
    
    /// <summary>
    /// The zero-based row index.
    /// </summary>
    public int RowIndex { get; }
    
    /// <summary>
    /// The column definition.
    /// </summary>
    public SbColumn<TItem> Column { get; }
    
    /// <summary>
    /// Creates a new cell context.
    /// </summary>
    public SbCellContext(TItem item, int rowIndex, SbColumn<TItem> column)
    {
        Item = item;
        RowIndex = rowIndex;
        Column = column;
    }
}
