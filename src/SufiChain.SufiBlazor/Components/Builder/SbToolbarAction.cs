namespace SufiChain.SufiBlazor.Components.Builder;

/// <summary>
/// Represents an action in an inline toolbar.
/// </summary>
public class SbToolbarAction
{
    /// <summary>
    /// Unique identifier for the action.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// Icon to display.
    /// </summary>
    public string? Icon { get; set; }
    
    /// <summary>
    /// Icon text (emoji or character).
    /// </summary>
    public string? IconText { get; set; }
    
    /// <summary>
    /// Display label.
    /// </summary>
    public string? Label { get; set; }
    
    /// <summary>
    /// Tooltip text.
    /// </summary>
    public string? Tooltip { get; set; }
    
    /// <summary>
    /// Whether the action is currently active/toggled.
    /// </summary>
    public bool IsActive { get; set; }
    
    /// <summary>
    /// Whether the action is disabled.
    /// </summary>
    public bool Disabled { get; set; }
    
    /// <summary>
    /// Whether this is a separator instead of an action.
    /// </summary>
    public bool IsSeparator { get; set; }
    
    /// <summary>
    /// Whether this is a danger action.
    /// </summary>
    public bool IsDanger { get; set; }
    
    /// <summary>
    /// Click handler.
    /// </summary>
    public Action? OnClick { get; set; }
    
    /// <summary>
    /// Creates a new toolbar action.
    /// </summary>
    public SbToolbarAction() { }
    
    /// <summary>
    /// Creates a new toolbar action with icon and tooltip.
    /// </summary>
    public SbToolbarAction(string icon, string? tooltip = null)
    {
        Icon = icon;
        Tooltip = tooltip;
    }
    
    /// <summary>
    /// Creates a separator.
    /// </summary>
    public static SbToolbarAction Separator() => new() { IsSeparator = true };
}
