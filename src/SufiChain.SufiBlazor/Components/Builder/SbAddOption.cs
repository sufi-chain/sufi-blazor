namespace SufiChain.SufiBlazor.Components.Builder;

/// <summary>
/// Represents an option in the add button menu.
/// </summary>
public class SbAddOption
{
    /// <summary>
    /// Unique identifier for the option.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// Display label for the option.
    /// </summary>
    public string Label { get; set; } = string.Empty;
    
    /// <summary>
    /// Icon to display.
    /// </summary>
    public string? Icon { get; set; }
    
    /// <summary>
    /// Icon text (emoji or character).
    /// </summary>
    public string? IconText { get; set; }
    
    /// <summary>
    /// Description or help text.
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Whether the option is disabled.
    /// </summary>
    public bool Disabled { get; set; }
    
    /// <summary>
    /// Optional category/group for the option.
    /// </summary>
    public string? Category { get; set; }
    
    /// <summary>
    /// Creates a new add option.
    /// </summary>
    public SbAddOption() { }
    
    /// <summary>
    /// Creates a new add option with label.
    /// </summary>
    public SbAddOption(string label, string? icon = null)
    {
        Label = label;
        Icon = icon;
    }
}
