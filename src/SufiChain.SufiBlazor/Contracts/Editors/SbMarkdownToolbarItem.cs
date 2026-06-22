namespace SufiChain.SufiBlazor.Contracts.Editors;

/// <summary>
/// Defines a toolbar item for the markdown editor.
/// </summary>
public class SbMarkdownToolbarItem
{
    public string Id { get; set; } = "";
    public string? Icon { get; set; }
    public string? IconName { get; set; }
    public string? Tooltip { get; set; }
    public string? Action { get; set; }
    public object? ActionValue { get; set; }
    public bool IsSeparator { get; set; }

    public static SbMarkdownToolbarItem Separator => new() { IsSeparator = true };
}

/// <summary>
/// Markdown editor content mode.
/// </summary>
public enum SbMarkdownEditorMode
{
    Markdown = 0,
    Source = 1
}
