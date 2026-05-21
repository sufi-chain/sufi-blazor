using Microsoft.AspNetCore.Components.Forms;

namespace SufiChain.SufiBlazor.Components.Data;

/// <summary>
/// Provides context for row editing operations in SbDataGrid.
/// </summary>
/// <typeparam name="TItem">The type of the data item being edited.</typeparam>
public class SbRowEditContext<TItem>
{
    /// <summary>
    /// Gets the original item before editing.
    /// </summary>
    public TItem OriginalItem { get; }
    
    /// <summary>
    /// Gets or sets the item being edited (a clone/copy for editing).
    /// </summary>
    public TItem EditItem { get; set; }
    
    /// <summary>
    /// Gets the row key.
    /// </summary>
    public string RowKey { get; }
    
    /// <summary>
    /// Gets the row index.
    /// </summary>
    public int RowIndex { get; }
    
    /// <summary>
    /// Gets the Blazor EditContext for validation.
    /// </summary>
    public EditContext EditContext { get; }
    
    /// <summary>
    /// Gets whether the edit is for a new row (insert mode).
    /// </summary>
    public bool IsNewRow { get; }
    
    /// <summary>
    /// Gets whether the item has been modified.
    /// </summary>
    public bool IsModified { get; set; }
    
    /// <summary>
    /// Creates a new row edit context.
    /// </summary>
    public SbRowEditContext(TItem originalItem, TItem editItem, string rowKey, int rowIndex, bool isNewRow = false)
    {
        OriginalItem = originalItem;
        EditItem = editItem;
        RowKey = rowKey;
        RowIndex = rowIndex;
        IsNewRow = isNewRow;
        EditContext = new EditContext(editItem!);
    }
    
    /// <summary>
    /// Validates the edit item.
    /// </summary>
    /// <returns>True if validation passes, false otherwise.</returns>
    public bool Validate()
    {
        return EditContext.Validate();
    }
    
    /// <summary>
    /// Gets validation messages for the edit item.
    /// </summary>
    public IEnumerable<string> GetValidationMessages()
    {
        return EditContext.GetValidationMessages();
    }
    
    /// <summary>
    /// Gets validation messages for a specific field.
    /// </summary>
    public IEnumerable<string> GetValidationMessages(string fieldName)
    {
        return EditContext.GetValidationMessages(new FieldIdentifier(EditItem!, fieldName));
    }
}
