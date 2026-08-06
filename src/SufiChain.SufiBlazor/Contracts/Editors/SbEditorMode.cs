namespace SufiChain.SufiBlazor.Contracts.Editors;

/// <summary>
/// Specifies the output format mode for the rich text editor.
/// </summary>
public enum SbEditorMode
{
    /// <summary>
    /// Output as HTML.
    /// </summary>
    Html,
    
    /// <summary>
    /// Output as Markdown.
    /// </summary>
    Markdown,
    
    /// <summary>
    /// Output as plain text (stripped of formatting).
    /// </summary>
    PlainText
}
