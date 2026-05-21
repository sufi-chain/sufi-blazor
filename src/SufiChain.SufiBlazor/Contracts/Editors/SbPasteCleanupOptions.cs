namespace SufiChain.SufiBlazor.Contracts.Editors;

/// <summary>
/// Options for cleaning up pasted content in the rich text editor.
/// </summary>
public class SbPasteCleanupOptions
{
    /// <summary>
    /// Whether to strip all formatting from pasted content.
    /// </summary>
    public bool StripAllFormatting { get; set; }
    
    /// <summary>
    /// Whether to clean up Word-specific HTML (MSO styles, etc.).
    /// </summary>
    public bool CleanWordHtml { get; set; } = true;
    
    /// <summary>
    /// Whether to remove inline styles.
    /// </summary>
    public bool RemoveInlineStyles { get; set; }
    
    /// <summary>
    /// Whether to remove CSS classes.
    /// </summary>
    public bool RemoveCssClasses { get; set; }
    
    /// <summary>
    /// Whether to remove empty elements.
    /// </summary>
    public bool RemoveEmptyElements { get; set; } = true;
    
    /// <summary>
    /// Whether to convert bold to strong, italic to em, etc.
    /// </summary>
    public bool NormalizeTags { get; set; } = true;
    
    /// <summary>
    /// List of allowed HTML tags. If null, all tags are allowed (after other cleanup).
    /// </summary>
    public string[]? AllowedTags { get; set; }
    
    /// <summary>
    /// List of allowed HTML attributes. If null, all attributes are stripped.
    /// </summary>
    public string[]? AllowedAttributes { get; set; }
    
    /// <summary>
    /// Maximum allowed depth for nested elements.
    /// </summary>
    public int MaxNestingDepth { get; set; } = 10;
    
    /// <summary>
    /// Creates default paste cleanup options.
    /// </summary>
    public static SbPasteCleanupOptions Default => new()
    {
        CleanWordHtml = true,
        RemoveEmptyElements = true,
        NormalizeTags = true
    };
    
    /// <summary>
    /// Creates strict paste cleanup options (plain text only).
    /// </summary>
    public static SbPasteCleanupOptions Strict => new()
    {
        StripAllFormatting = true
    };
}
