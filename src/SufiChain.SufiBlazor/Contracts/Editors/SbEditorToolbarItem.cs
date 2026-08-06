namespace SufiChain.SufiBlazor.Contracts.Editors;

/// <summary>
/// Defines a toolbar item for the rich text editor.
/// </summary>
public class SbEditorToolbarItem
{
    /// <summary>
    /// The unique identifier for the toolbar item.
    /// </summary>
    public string Id { get; set; } = "";
    
    /// <summary>
    /// The type of toolbar item.
    /// </summary>
    public SbEditorToolbarItemType Type { get; set; } = SbEditorToolbarItemType.Button;
    
    /// <summary>
    /// The icon to display (emoji, icon class, or HTML).
    /// When <see cref="IconName"/> is set, this is used as fallback only.
    /// </summary>
    public string? Icon { get; set; }
    
    /// <summary>
    /// Optional Sufi icon name (e.g. "italic", "list"). When set, the toolbar renders <c>SbIcon</c> with this name instead of <see cref="Icon"/>.
    /// </summary>
    public string? IconName { get; set; }
    
    /// <summary>
    /// The tooltip text.
    /// </summary>
    public string? Tooltip { get; set; }
    
    /// <summary>
    /// The Quill format name (e.g., "bold", "italic", "list").
    /// </summary>
    public string? Format { get; set; }
    
    /// <summary>
    /// The format value (for formats that require a value, like "list": "ordered").
    /// </summary>
    public object? FormatValue { get; set; }
    
    /// <summary>
    /// Options for dropdown/select items.
    /// </summary>
    public IReadOnlyList<SbEditorToolbarOption>? Options { get; set; }
    
    /// <summary>
    /// Whether this is a separator.
    /// </summary>
    public bool IsSeparator { get; set; }
    
    /// <summary>
    /// Custom action handler name (for custom buttons).
    /// </summary>
    public string? CustomAction { get; set; }
    
    // Factory methods for common toolbar items
    
    public static SbEditorToolbarItem Bold => new() { Id = "bold", Format = "bold", IconName = "bold", Icon = "B", Tooltip = "Bold (Ctrl+B)" };
    public static SbEditorToolbarItem Italic => new() { Id = "italic", Format = "italic", IconName = "italic", Icon = "I", Tooltip = "Italic (Ctrl+I)" };
    public static SbEditorToolbarItem Underline => new() { Id = "underline", Format = "underline", IconName = "underline", Icon = "U", Tooltip = "Underline (Ctrl+U)" };
    public static SbEditorToolbarItem Strike => new() { Id = "strike", Format = "strike", Icon = "S", Tooltip = "Strikethrough" };
    
    public static SbEditorToolbarItem OrderedList => new() { Id = "ol", Format = "list", FormatValue = "ordered", Icon = "1.", Tooltip = "Numbered List" };
    public static SbEditorToolbarItem BulletList => new() { Id = "ul", Format = "list", FormatValue = "bullet", IconName = "list", Icon = "•", Tooltip = "Bullet List" };
    
    public static SbEditorToolbarItem Link => new() { Id = "link", Format = "link", Icon = "🔗", Tooltip = "Insert Link" };
    public static SbEditorToolbarItem Image => new() { Id = "image", Format = "image", Icon = "🖼️", Tooltip = "Insert Image" };
    
    public static SbEditorToolbarItem Blockquote => new() { Id = "blockquote", Format = "blockquote", Icon = "❝", Tooltip = "Quote" };
    public static SbEditorToolbarItem CodeBlock => new() { Id = "code-block", Format = "code-block", Icon = "</>" , Tooltip = "Code Block" };
    
    public static SbEditorToolbarItem Clean => new() { Id = "clean", CustomAction = "clean", Icon = "✕", Tooltip = "Clear Formatting" };
    
    public static SbEditorToolbarItem Separator => new() { IsSeparator = true };
    
    public static SbEditorToolbarItem Header => new()
    {
        Id = "header",
        Type = SbEditorToolbarItemType.Select,
        Format = "header",
        Tooltip = "Heading",
        Options = new List<SbEditorToolbarOption>
        {
            new() { Label = "Normal", LabelKey = "Rte:HeaderNormal", Value = false },
            new() { Label = "Heading 1", LabelKey = "Rte:Header1", Value = 1 },
            new() { Label = "Heading 2", LabelKey = "Rte:Header2", Value = 2 },
            new() { Label = "Heading 3", LabelKey = "Rte:Header3", Value = 3 },
            new() { Label = "Heading 4", LabelKey = "Rte:Header4", Value = 4 },
            new() { Label = "Heading 5", LabelKey = "Rte:Header5", Value = 5 },
            new() { Label = "Heading 6", LabelKey = "Rte:Header6", Value = 6 },
        }
    };
    
    public static SbEditorToolbarItem Align => new()
    {
        Id = "align",
        Type = SbEditorToolbarItemType.Select,
        Format = "align",
        Tooltip = "Alignment",
        Options = new List<SbEditorToolbarOption>
        {
            new() { Label = "Left", LabelKey = "Rte:AlignLeft", Value = false },
            new() { Label = "Center", LabelKey = "Rte:AlignCenter", Value = "center" },
            new() { Label = "Right", LabelKey = "Rte:AlignRight", Value = "right" },
            new() { Label = "Justify", LabelKey = "Rte:AlignJustify", Value = "justify" },
        }
    };
    
    public static SbEditorToolbarItem Undo => new() { Id = "undo", CustomAction = "undo", Icon = "↶", Tooltip = "Undo (Ctrl+Z)" };
    public static SbEditorToolbarItem Redo => new() { Id = "redo", CustomAction = "redo", Icon = "↷", Tooltip = "Redo (Ctrl+Y)" };
}

/// <summary>
/// The type of toolbar item.
/// </summary>
public enum SbEditorToolbarItemType
{
    /// <summary>
    /// A toggle button.
    /// </summary>
    Button,
    
    /// <summary>
    /// A dropdown select.
    /// </summary>
    Select,
    
    /// <summary>
    /// A color picker.
    /// </summary>
    ColorPicker
}

/// <summary>
/// An option for dropdown toolbar items.
/// </summary>
public class SbEditorToolbarOption
{
    /// <summary>
    /// The display label. Used when <see cref="LabelKey"/> is null.
    /// </summary>
    public string Label { get; set; } = "";
    
    /// <summary>
    /// Localization key (e.g. "Rte:HeaderNormal"). When set, the label is resolved via IStringLocalizer.
    /// </summary>
    public string? LabelKey { get; set; }
    
    /// <summary>
    /// The value to apply.
    /// </summary>
    public object? Value { get; set; }
}
