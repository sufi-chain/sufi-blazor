namespace SufiChain.SufiBlazor.Components.Navigation;

/// <summary>
/// Represents a breadcrumb item.
/// </summary>
public class SbBreadcrumbItem
{
    /// <summary>
    /// Display text for the breadcrumb.
    /// </summary>
    public string Text { get; set; } = string.Empty;
    
    /// <summary>
    /// URL for the breadcrumb link. If null, item is not clickable.
    /// </summary>
    public string? Href { get; set; }
    
    /// <summary>
    /// Optional icon to display.
    /// </summary>
    public string? Icon { get; set; }
    
    /// <summary>
    /// Creates a new breadcrumb item.
    /// </summary>
    public SbBreadcrumbItem() { }
    
    /// <summary>
    /// Creates a new breadcrumb item with text and optional href.
    /// </summary>
    public SbBreadcrumbItem(string text, string? href = null)
    {
        Text = text;
        Href = href;
    }
}
